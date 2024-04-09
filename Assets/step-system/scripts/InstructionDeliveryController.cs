using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;

/// <summary>
/// Delivers instructions to the user via Unity Timeline. Instruction delivery has 
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class InstructionDeliveryController : MonoBehaviour
{
    public bool ShouldPlayOnStart; // Should these instructions be delivered when the game starts?
    
    [SerializeField] private UnityEvent onInstructionsDelivered = new UnityEvent(); // event that fires when instructions are done
    [SerializeField] private UnityEvent OnInstructionsStarted = new UnityEvent(); // event that fires when the instructions have started
    

    [TextArea(2, 10)]
    public string description; // This will hold the component-specific details
    public TimelineAsset TimelineAsset;
    [SerializeField] private string script; // What should the instructions say?//

    private PlayableDirector _director; // This will play our timeline to deliver the instruction.
    private float _instructionDuration; // How long is the timeline asset?
    private bool _isUserReceivingInstruction; // State of instructions - are the instructions currently being delivered
    private float _instructionCurrentTime; // State of instructions - what time point are the instructions at

    private bool _paused; // are instructions paused?
    private bool _nextOrBackClicked; 
    private bool _loopClicked; // should we loop?

    public InstructionDeliveryMetadata MetaData;

    /// <summary>
    /// Public getter for isUserReceivingInstruction.
    /// </summary>
    public bool IsUserReceivingInstruction => _isUserReceivingInstruction;

    /// <summary>
    /// Public getter for pause state of PlayableDirector.
    /// </summary>
    public bool Paused => _paused;

    /// <summary>
    /// Public getter for instructionDuration.
    /// </summary>
    public float InstructionDuration => _instructionDuration;

    /// <summary>
    /// Public getter for InstructionCurrentTime.
    /// </summary>
    public float InstructionCurrentTime => _instructionCurrentTime;

    /// <summary>
    /// Public getter for the script.
    /// </summary>
    public string Script => script;

    /// <summary>
    /// Awake is responsible for setting up the InstructionDeliveryController.
    /// </summary>
    private void Awake()
    {
        // Get the director on the game object. This will never fail because we require that any gameobject
        // that has the InstructionDeliveryComponent will automatically add the PlayableDirector component.
        _director = GetComponent<PlayableDirector>();

        // Set the instructionDuration based on the TimelineAsset for the instructions
        if(TimelineAsset != null)
        {
            _instructionDuration = (float)TimelineAsset.duration;
        }
    }

    /// <summary>
    /// Check to see if the instructions should be delivered when the game starts and deliver them if so.
    /// </summary>
    private void Start()
    {
        if (ShouldPlayOnStart) 
        {
            StartInstructionDelivery();
        }
    }

    private void OnEnable()
    {
        // setup director subscribers
        _director.paused += _directorPaused;
        _director.played += _directorPlayed;
    }

    private void OnDisable()
    {
        // remove director subscribers,
        // avoid that garbage collection
        _director.paused -= _directorPaused;
        _director.played -= _directorPlayed;
    }

    /// <summary>
    /// Called by next and back buttons in InstructionDeliveryListController.
    /// Ticks flag nextOrBackClicked in order to break out of coroutine.
    /// </summary>
    public void BreakOutOfCurrentStep()
    {
        _director.Pause(); // ensure audio does not overlap
        _nextOrBackClicked = true;
        _loopClicked = false; // can change if want default state to be looping steps unless user specifies otherwise
    }

    /// <summary>
    /// Called by pause/play button in InstructionDeliveryListController.
    /// Ensures that nextOrBackClicked flag is not ticked, only pauses director.
    /// </summary>
    public void PauseInstructions()
    {
        _director.Pause();
    }

    public void ResumeInstructions()
    {
        // if played all the way through, restart coroutine
        if (!_isUserReceivingInstruction)
        {
            _deliverInstructions();
        }
        else
        {
            // otherwise, just unpause director to not lose place in timeline
            _director.Play();
        }
    }

    public void ResetInstructions()
    {
        // set time to zero, bring to first frame 
        _director.time = 0f;
        // set to false, ResumeInstructions() will StartCoroutine from top
        _isUserReceivingInstruction = false;
        // ensure audio does not overlap
        _director.Pause();
    }

    /// <summary>
    /// Called by LoopInstructions in InstructionDeliveryListController.
    /// Ticks loopClicked flag accordingly.
    /// </summary>
    public void LoopInstructions()
    {
        if (!_loopClicked)
        {
            _loopClicked = true;
        }
        else
        {
            _loopClicked = false;
        }
    }

    /// <summary>
    /// Called by OnSliderValueChanged in InstructionDeliveryListController.
    /// Sets PlayableDirector.time equal to value that was scrubbed to.
    /// </summary>
    /// <param name="value"> Converted instructionTime value to fit slider. </param>
    public void ScrubDirector(float value)
    {
        ResumeInstructions();
        _instructionCurrentTime = value;
        _director.time = _instructionCurrentTime;
        _director.Evaluate();
        _director.Play();
    }

    /// <summary>
    /// Wrapper to start a coroutine to deliver instructions. Can be called from any script to start instruction delivery.
    /// </summary>
    public void StartInstructionDelivery()
    {
        _deliverInstructions();
    }

    private void _directorPlayed(PlayableDirector obj)
    {
        _paused = false;
    }

    private void _directorPaused(PlayableDirector obj)
    {
        _paused = true;
    }

    /// <summary>
    /// Creates a loop for instruction delivery. This loop runs while the instructions are being delivered and ticks forward 
    /// the instructions current time which is used to determine when the instructions are finished. This function is meant 
    /// to be used as a coroutine. 
    /// </summary>
    private void _deliverInstructions()
    {
        void _errorHandling()
        {
            // If you don't have a timeline, break
            if (!TimelineAsset)
            {
                Debug.LogWarning($"[InstructionDeliveryController {name}- Warning] You are trying to deliver instructions but do NOT have a timeline.");
                return;
            }
        }

        void _resetState()
        {
            // Make sure time is at zero when coroutine started
            _instructionCurrentTime = 0f;
            // Make sure flag is reset 
            _nextOrBackClicked = false;
        }

        void _startPlaying()
        {
            // Start the playing process
            _isUserReceivingInstruction = true;
            _director.playableAsset = TimelineAsset;
            _director.Play();
        }

        IEnumerator _instructionLoop()
        {
            // Loop that ticks instruction current time while the user is receiving instructions.
            while (_isUserReceivingInstruction)
            {
                Debug.Log("Coroutine pause status: " + _paused);
                if (!_paused)
                {
                    _tickStepInstructionCurrentTime(Time.deltaTime);
                    yield return null;
                }
                else
                { // paused, when next or back are clicked StopCurrentCoroutine() pauses director
                    if (_nextOrBackClicked)
                    {
                        yield break; // if next or back pressed, break out of current step 
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }

            // Step was played all the way through
            _director.time = 0f; // reset director to beginning
            yield break; // stop coroutine
        }

        // ensure we are ready to deliver instructions
        _errorHandling();

        // reset the 
        _resetState();

        _startPlaying();

        // OnInstructionsStarted UnityEvent activated
        OnInstructionsStarted.Invoke();

        StartCoroutine(_instructionLoop());
    }

    /// <summary>
    /// Just increments the instruction current time by the a delta time.
    /// </summary>
    /// <param name="deltaTime"> Amount to tick by. </param>
    private void _tickStepInstructionCurrentTime(float deltaTime)
    {
        // increment
        _instructionCurrentTime += deltaTime;

        Debug.Log("Instruction current time: " + _instructionCurrentTime);

        // If current time is greater than or equal to the instruction duration,
        // the instructions have been delivered.
        if (_instructionCurrentTime >= _instructionDuration)
        {
            if (!_loopClicked)
            {
                Debug.Log("Delivered Invoked." + name);
                onInstructionsDelivered.Invoke(); // OnInstructionsDelivered UnityEvent activated
                _director.Pause();
                Debug.Log("Pause status: " + _paused);
                _isUserReceivingInstruction = false;
            }
            else
            { // loop clicked, start coroutine again at end of sequence
                StopAllCoroutines();
                ResetInstructions();
                ResumeInstructions();
            }
        }
    }

    
}

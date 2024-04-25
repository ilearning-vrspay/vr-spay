using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages steps in a procedure. Steps are manually dragged in through the inspector
/// and public List is populated. 
/// Functions below are called upon user interaction with UI elements such as Buttons or Sliders.
/// Functions invoke dynamic UnityEvents that can control UI elements or other conditions. 
/// </summary>

public class InstructionDeliveryListController : MonoBehaviour
{
    // Populated through inspector
    public List<InstructionDeliveryController> Steps = new List<InstructionDeliveryController>();
    public int StepIndex;

    // Events can take in a current instance of InstructionDeliveryListController by passing in "this".
    public UnityEvent<InstructionDeliveryListController> SequenceStarted = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent<InstructionDeliveryListController> SequencePlaying = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent<InstructionDeliveryListController> SequenceNext = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent<InstructionDeliveryListController> SequenceBack = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent<InstructionDeliveryListController> SequenceReset = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent<InstructionDeliveryListController> SequenceScrubbed = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent SequencePaused = new UnityEvent();
    public UnityEvent SequenceResumed = new UnityEvent();
    public UnityEvent<InstructionDeliveryListController> ProgramMuted = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent<InstructionDeliveryListController> ProgramUnmuted = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent<InstructionDeliveryListController> InstructionDelivered = new UnityEvent<InstructionDeliveryListController>();
    public UnityEvent ProgramRestarted = new UnityEvent();
    
    private bool _startClicked;
    private bool _adjustingProgressBar;
    private bool _isMuted;

    private float _sliderValueNonConverted;
    private float _sliderValueConverted;

    /// <summary>
    /// Public getter for each step's respective InstructionDeliveryController.
    /// </summary>
    public InstructionDeliveryController GetCurrentInstructionDeliveryController()
    {
        if (Steps[StepIndex] == null)
        {
            Debug.Log("Not a valid index!");
            return null;
        }
        return Steps[StepIndex];
    }

    /// <summary>
    /// Scrubber dynamically moves with instruction time.
    /// </summary>
    private void Update()
    {
        //Debug.Log("at index: " + StepIndex + "named: " + Steps[StepIndex].name);

        if (!_adjustingProgressBar)
        {
            SequencePlaying.Invoke(this);
        }
    }


    private void OnEnable()
    {
        foreach(var step in Steps)
        {
            step.onInstructionsDelivered.AddListener(OnInstructionDelivered);
        }
    }

    private void OnDisable()
    {
        foreach (var step in Steps)
        {
            step.onInstructionsDelivered.RemoveListener(OnInstructionDelivered);
        }
    }

    private void OnInstructionDelivered()
    {
        InstructionDelivered.Invoke(this);
    }

    /* Functions below are called during onClick() of respective buttons */

    public void StartSequence()
    {
        if (!_startClicked)
        {
            StepIndex = 0;
            GetCurrentInstructionDeliveryController().ResetInstructions();
            SequenceStarted.Invoke(this);
            GetCurrentInstructionDeliveryController().ResumeInstructions();
            _startClicked = true;
        }
        else
        {
            GetCurrentInstructionDeliveryController().ResetInstructions();
            ProgramRestarted.Invoke();
            _startClicked = false;
        }
        Debug.Log("Paused: " + GetCurrentInstructionDeliveryController().Paused);
    }

    public void StartNextStep()
    {
        // handle the case when we are on the last step and try to go next
        if (StepIndex + 1 >= Steps.Count) return;

        // pause current step before moving on (incrementing step index)
        GetCurrentInstructionDeliveryController().ResetInstructions();
        // breaks out of current coroutine
        GetCurrentInstructionDeliveryController().BreakOutOfCurrentStep();

        StepIndex++;
        SequenceNext.Invoke(this);
        GetCurrentInstructionDeliveryController().StartInstructionDelivery();

        Debug.Log("Paused: " + GetCurrentInstructionDeliveryController().Paused);
    }

    public void StartPreviousStep()
    {
        // pause current step before going back (decrementing step index)
        GetCurrentInstructionDeliveryController().ResetInstructions();
        // breaks out of current coroutine
        GetCurrentInstructionDeliveryController().BreakOutOfCurrentStep();

        StepIndex--;
        SequenceBack.Invoke(this);
        GetCurrentInstructionDeliveryController().StartInstructionDelivery();

        Debug.Log("Paused: " + GetCurrentInstructionDeliveryController().Paused);
    }

    public void ResetStep()
    {
        GetCurrentInstructionDeliveryController().ResetInstructions();
        SequenceReset.Invoke(this);
        Debug.Log("Paused: " + GetCurrentInstructionDeliveryController().Paused);
    }

    public void PauseAndResumeStep()
    {
        // If currently playing, pause instructions when pause button clicked
        if (!GetCurrentInstructionDeliveryController().Paused)
        {
            GetCurrentInstructionDeliveryController().PauseInstructions();
            SequencePaused.Invoke();
            Debug.Log("Paused: " + GetCurrentInstructionDeliveryController().Paused);
        }
        else
        {
            // If currently paused, play instructions when play button clicked
            GetCurrentInstructionDeliveryController().ResumeInstructions();
            SequenceResumed.Invoke();
            Debug.Log("Paused: " + GetCurrentInstructionDeliveryController().Paused);
        }
    }

    /// <summary>
    /// Controls volume of program. Volume is on when AudioListener.volume = 1. 
    /// Ticks isMuted flag accordingly.
    /// </summary>
    public void VolumeControl()
    {
        if (!_isMuted)
        {
            AudioListener.volume = 0;
            _isMuted = true;
            ProgramMuted.Invoke(this);
        }
        else
        {
            AudioListener.volume = 1;
            _isMuted = false;
            ProgramUnmuted.Invoke(this);
        }
    }

    public void LoopStep()
    {
        GetCurrentInstructionDeliveryController().LoopInstructions();
    }

    public void SkipInstructions()
    {
        GetCurrentInstructionDeliveryController().ScrubDirector(GetCurrentInstructionDeliveryController().InstructionDuration * 0.99f);
    }
}

## Contributing Guide

Follow these steps to contribute to the project.

### Phase 1: One-Time Setup
*Do this only once when you first join the project.*

1.  **Configure Access:** Ensure your SSH keys or Personal Access Tokens are set up for GitHub/GitLab.
2.  **Clone the Repository:**
    
    ```bash
    git clone <repository-url>
    cd <project-folder>
    ```

---

### Phase 2: The Development Cycle
*Repeat these steps for every new feature or bug fix.*

#### 1. Sync and Branch
Always start from a fresh, updated state to avoid conflicts later.
```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```
#### 2. Make Changes
Write your code. Once you are satisfied, stage your changes.

```bash
git add .
```

#### 3. Commit
Write a clear, descriptive message about what you changed.

```Bash
git commit -m "feat: Add brief description of change"
```

4. Sync Again (Crucial Step!)
Before pushing, check if others have made changes to main while you were working. This prevents messy merge conflicts.

```Bash
git fetch origin
git rebase origin/main
# If conflicts arise, resolve them, then run: git rebase --continue
-OR-
git merge origin/main (messy option but gets job done, but you stil have to navigate through merge conflicts)
```

#### 5. Push
Push your branch to the remote repository.

```Bash
git push -u origin feature/your-feature-name
```
#### 6. Create Pull Request
Go to the repository online and open a Pull Request (PR) against the main branch.

Ready for the next task? Go back to Step 1 of Phase 2 to start your next feature!

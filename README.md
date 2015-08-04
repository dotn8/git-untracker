# git-untracker

This is `git-untracker`, a cross-platform Git extension in the form of a couple of git hooks and two new git commands: `git untrack` and `git retrack`. `git-untracker` uses a file called `.gituntrack` which is similar to `.gitignore` but it works only on tracked files, whereas `.gitignore` only works on *un*tracked files.

Before you can decide whether or not you need this software, you should understand Git, Git hooks, and what `.gitignore` does.

**Note:** if you have configured the `%GIT_TEMPLATE_DIR%` to be something custom, please be aware that using the installer for this program will overwrite the system `%GIT_TEMPLATE_DIR%`.

## Use cases

### App.config as a manual template

Jim's company has a central Git repository in which there is a Visual Studio project. This project contains an `App.config` file that stores a path that is different for each developer; Jim's local clone of the repository has an `App.config` file that looks like this:

    <?xml version="1.0" encoding="utf-8"?>

    <configuration>
    ...
      <UserSpecifiedBackupDirectory>
        C:\Users\Jim\Documents\Backups
      </UserSpecifiedBackupDirectory>
    ...
    </configuration>

Jim has a coworker named Ellie. She also has a local clone of the repository with an `App.config` that looks like this:

    <?xml version="1.0" encoding="utf-8"?>

    <configuration>
    ...
      <UserSpecifiedBackupDirectory>
        C:\Users\Ellie\Dropbox\Work\Backups
      </UserSpecifiedBackupDirectory>
    ...
    </configuration>

The problem arises when Jim or Ellie commit `App.config` and push that commit to the central repository. The next time the other person pulls from the central repository, git sounds a conflict alarm that says, "We should use either Ellie's or Jim's version of `App.config`." But that requirement is annoying to everyone because Ellie can't use Jim's version and Jim can't use Ellie's version.

`git-untracker` solves this problem. In this use case, both Jim and Ellie would download and install `git-untracker` and then run `git init` on the command line in the root of all their local copies of the repository. Then one of them (let's assume it's Jim for sake of argument) would start untracking the `App.config` file by running the following command line in a command prompt in the repository:

    git untrack TheProject/App.config

Jim would then commit the newly created `TheProject/.gituntrack` and push it to the central repository. Ellie would pull that change down. Now, they can make changes to `App.config` and git will never commit those changes. They don't have to worry about `App.config` conflicts anymore.

Whenever Jim or Ellie want to make a change to the committed version of the file, they can temporarily retrack the file (like this: `git retrack TheProject/App.config --temporary`), revert uncommitted changes (like this: `git checkout App.config`), change the file, commit it, and then untrack the file again manually (like this: `git untrack TheProject/App.config`). This way, the copy of `App.config` in the git history serves as a **template**, to be filled out by a developer on a fresh clone.

Also, Jim and Ellie design the committed `App.config` so that if a developer *doesn't* customize it to his or her environment, the program will immediately crash. This way, their configuration is designed to never fail silently (the program crashes if they haven't customized `App.config`) and follow the principle of least surprise (the program does not commit the customized `App.config`).

Note: the purpose of this use case is not to justify this software. It's to describe a simple problem and solution that potential users can identify with.

## How to install

### On Windows

1. Download the latest installer from [here](https://github.com/JohnBillington/git-untracker/releases) and run it.
2. In each git repository that you want `git-untrack` to work in, copy the files from `C:\Program Files (x86)\git-untracker\git_template_dir\hooks` to `my_repository\.git\hooks`.
3. Restart any terminal windows in which you're using `git`. This is needed because the installer modifies the `%PATH%` and relies on the updated `%PATH%` to work.

### Notes

**Note**: new repositories will automatically have `git-untrack` enabled in them.

**Note**: at least one IDE (Visual Studio 2015), when creating a new project, will give the user the option of creating a new git repository to put the new project in. In these cases, it's possible that the created git repository will have been created with a custom template, in which case `git-untracker` will not be installed in that repository. You can install `git-untracker` into the new repository manually by running `git init` in its root directory or by copying the files from `C:\Program Files (x86)\git-untracker\git_template_dir\hooks` into the `.git/hooks` folder of the new repository.

## How to build installer

[Visual Studio](https://www.visualstudio.com/downloads/download-visual-studio-vs) and [WiX](http://wixtoolset.org/) must be installed to build the installer.

1. Build the projects in Visual Studio with configuration `Release` and platform `AnyCPU`
2. Clone the `git-untracker` repository: `git clone "https://github.com/JohnBillington/git-untracker.git"`
3. Open a command prompt and `cd` into the root of the `git-untracker` repository
4. Run `"C:\Program Files (x86)\WiX Toolset v3.9\bin\candle.exe" -nologo "installer\git-untracker.wxs" -out "installer\git-untracker.wixobj"`
5. Run `"C:\Program Files (x86)\WiX Toolset v3.9\bin\light.exe" -nologo "installer\git-untracker.wixobj" -out "installer\git-untracker.msi"`

## FAQ

## What does `.gituntrack` do if I do have `git-untracker` installed?

1. If a tracked file is listed in `.gituntrack` then that file's changes will never be committed.
2. Files can be removed from `.gituntrack` and added to it; when `.gituntrack` is committed with these changes, these changes will be applied to the git repository.

### What does `.gituntrack` do if I don't have `git-untracker` installed?

Nothing. If you don't have `git-untracker` installed, then the `.gituntrack` file has no special meaning.

### How do I untrack a file?

    git untrack path/to/file.txt

**Note:** the path is case sensitive, even on Windows.

### How do I re-track a file?

    git retrack path/to/file.txt

**Note:** the path is case sensitive, even on Windows.

### How do I temporarily force a file to be tracked?

To temporarily force a file to be tracked, run the following command line:

    git retrack path/to/file.txt --temporary

To undo the temporary tracking of a file, run the following command line:

    git untrack -c

You can run the above command at any time to make sure all the files are properly tracked or untracked depending upon the contents of `.gituntrack` files.

To see which files are untracked, run the following command line (from [here](http://stackoverflow.com/a/2363495/4995014)):

    git ls-files -v | grep '^[[:lower:]]'

**Note:** the paths are case sensitive, even on Windows.

### If I've made local changes to the untracked file and I attempt to commit, will I see the file in the list of changed files?

No. If you do, that means that `git-untracker` is not installed or it is not installed in your git repository. See the "How to install" section to fix this.

### What happens when there *aren't* local changes to the untracked file and I pull from a remote that has an updated version of the file?

Your local file will be changed to match the updated file.

### What happens when there *are* local changes to the untracked file and I pull from a remote that has an updated version of the file?

When the person with the local changes pulls from the remote that has an updated version of the file, `git` detects that there's a conflict and displays a message like this:

    λ git pull origin
    remote: Counting objects: 7, done.
    remote: Compressing objects: 100% (4/4), done.
    remote: Total 4 (delta 3), reused 0 (delta 0)
    Unpacking objects: 100% (4/4), done.
    From C:\Users\John\Documents\git-untracker
       2a8b992..e8c3bd5  master     -> origin/master
    Updating 2a8b992..e8c3bd5
    error: Your local changes to the following files would be overwritten by merge:
            i-am-untracked.txt
    Please, commit your changes or stash them before you can merge.
    Aborting

In other words, `git` will not overwrite your local changes; it will force you to resolve the conflict.

**However** when you try to resolve the conflicts, `git` will show no conflicts. Currently you must temporarily start tracking a file again using the trick shown in the FAQ `How do I temporarily force a file to be tracked?`, resolve the conflict, and then revoke the temporary tracking of the file as described in the same FAQ.

### Should I use Windows-style paths or Unix-style paths in `.gituntrack`?

You can use either.

### Can I put a `.gituntrack` file in sub-folders in the repository, or does it have to be in the repository root?

`.gituntrack` can be anywhere in the repository. Paths must always be relative to the directory containing the `.gituntrack` file.

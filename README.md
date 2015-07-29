# git-untracker

This is `git-untracker`, a cross-platform Git extension in the form of a couple of git hooks. `git-untracker` uses a file called `.gituntrack` which is similar to `.gitignore` but it works only on tracked files, whereas `.gitignore` only works on *un*tracked files.

Before you can decide whether or not you need this software, you should understand Git hooks and what `.gitignore` does.

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

`git-untracker` solves this problem. In this use case, both Jim and Ellie would download and install `git-untracker` and then run `git init` on the command line in the root of all their local copies of the repository. Then one of them (let's assume it's Jim for sake of argument) would add a file named `.gituntrack` in the root of the repository, and in that would would be one line:

    TheProject/App.config

Jim would then commit this file and push it to the central repository. Ellie would pull that change down. Now, they can make changes to `App.config` and git will never commit those changes. They don't have to worry about `App.config` conflicts anymore.

Whenever Jim or Ellie want to make a change to the committed version of the file, they can temporarily track the file (see the FAQ), revert uncommitted changes (like this: `git checkout App.config`), change the file, commit it, and then untrack the file again manually (see the FAQ). This way they can have an `App.config` that is a **template** to be filled out by a developer on a fresh clone.

Also, Jim and Ellie design the committed `App.config` so that if a developer *doesn't* customize it to his or her environment, the program will immediately crash. This way, their configuration is designed to never fail silently (the program crashes if they haven't customized `App.config`) and follow the principle of least surprise (the program does not commit the customized `App.config`).

Note: the purpose of this use case is not to justify this software. It's to describe a simple problem and solution that potential users can identify with.

## How to install

1. Download the git-untracker repository: `git clone https://github.com/JohnBillington/git-untracker.git`
2. Make the `git_template_dir` the default git template like this: `git config --global init.templateDir path/to/git-untracker/git_template_dir`
3. Any pre-existing git repositories must have `git init` executed in their root in order to add `git-untracker` functionality to them. New repositories will automatically have `git-untracker` installed in them (with one exception noted below).

Note: at least one IDE (Visual Studio 2015), when creating a new project, will give the user the option of creating a new git repository to put the new project in. In these cases, it's possible that the created git repository will have been created with a custom template, in which case `git-untracker` will not be installed in that repository. You can install `git-untracker` into the new repository manually by running `git init` in its root directory or by copying the hooks from *this* repository's `hooks` folder into the `.git/hooks` folder of the new repository.

### How to verify installation

1. Edit and save changes to the file `i-am-untracked.txt`
2. Try to commit *everything* either via the command line (`git commit -a`) or via your favorite git user interface. If and only if `i-am-untracked.txt` does not show up as changed, then `git-untracker` is installed successfully.

## Faq

## What does `.gituntrack` do?

1. If a tracked file is listed in `.gituntrack` then that file's changes will never be committed.
2. Files can be removed from `.gituntrack` and added to it; when `.gituntrack` is committed with these changes, these changes will be applied to the git repository.

### How do I untrack a file?

1. Make sure the file has been committed.
2. Add the file path to `.gituntrack`. The path must be relative to the root of the repository. File name patterns (e.g., `*.txt`) are not supported.
3. Commit the changes to `.gituntrack`. Be aware: the file stops being tracked *after* `.gituntrack` is committed with the file name inside it. So, if there are any uncommitted changes in the file when `.gituntrack` is being committed, `git` will try to commit the file as well. In other words, there may be one last commit to the file in question.

### What does `.gituntrack` do if I don't have `git-untracker` installed?

Nothing. If you don't have `git-untracker` installed, then the `.gituntrack` file has no special meaning.

### How do I temporarily force a file to be tracked?

To temporarily force a file to be tracked, run the following command line:

    git update-index --no-assume-unchanged path/to/file

To stop tracking a file manually, run the following command line:

    git update-index --assume-unchanged path/to/file

To see which files are untracked, run the following command line (from [here](http://stackoverflow.com/a/2363495/4995014)):

    git ls-files -v | grep '^[[:lower:]]'

### What happens when there are local changes to the untracked file and remote, committed changes to the untracked file?

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

## Pitfalls

1. Right now the `.gituntrack` file must be in the repository root.
2. Some tools automatically specify their own template when creating a git repository; in these cases, the user must manually add the hooks to the repository. See the section `How to manually add to an existing repository` for more info.
# git-untracker

This is `git-untracker`, a cross-platform Git extension in the form of a couple of git hooks. `git-untracker` uses a file called `.gituntrack` which is similar to `.gitignore` but it works only on tracked files, whereas `.gitignore` only works on untracked files.

Before you can decide whether or not you need this software, you should understand Git hooks and what `.gitignore` does.

## Use cases

### App.config

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

Note: the purpose of this use case is not to justify this software. It's to describe a simple problem and solution that potential users can identify with.

## What does `.gituntrack` do

1. If a tracked file is listed in `.gituntrack` then that file's changes will never be committed.
2. Files can be removed from `.gituntrack` and added to it; when `.gituntrack` is committed with these changes, these changes will be applied to the git repository.

## How to install

1. Download the git-untracker repository: `git clone https://github.com/JohnBillington/git-untracker.git`
2. Make the `git_template_dir` the default git template like this: `git config --global init.templateDir path/to/git-untracker/git_template_dir`
3. Any pre-existing git repositories must have `git init` executed in their root in order to add `git-untracker` functionality to them. New repositories will automatically have `git-untracker` installed in them (with one exception noted below).

Note: at least one IDE (Visual Studio 2015), when creating a new project, will give the user the option of creating a new git repository to put the new project in. In these cases, it's possible that the created git repository will have been created with a custom template, in which case `git-untracker` will not be installed in that repository. You can install `git-untracker` into the new repository manually by running `git init` in its root directory or by copying the hooks from *this* repository's `hooks` folder into the `.git/hooks` folder of the new repository.

### How to verify installation

1. Edit and save changes to the file `i-am-untracked.txt`
2. Try to commit *everything* either via the command line (`git commit -a`) or via your favorite git user interface. If and only if `i-am-untracked.txt` does not show up as changed, then `git-untracker` is installed successfully.

## How to add to an existing repository

### Option 1

This option is probably easier for Windows users. All you need to do is copy each file from the `hooks` directory in this repository to the `.git/hooks` directory in the repository that needs `git-untracker`.

### Option 2

1. Make sure `git-untracker` is installed
2. In the root of the repository in question, run `git init`. Running `git init` again does not erase the repository contents, it serves mostly just to update anything that changed in the git template.

## How to untrack a file

1. Make sure the file has been committed.
2. Add the file path to `.gituntrack`. The path must be relative to the root of the repository. File name patterns are not supported.
3. Commit the changes to `.gituntrack`. Be aware: the file stops being tracked *after* `.gituntrack` is committed with the file name inside it. So, if there are any uncommitted changes in the file when `.gituntrack` is being committed, `git` will try to commit the file as well. In other words, there may be one last commit to the file if you commit all your changes.

## Pitfalls

1. Right now the `.gituntrack` file must be in the repository root.
2. Some tools automatically specify their own template when creating a git repository; in these cases, the user must manually add the hooks to the repository. See the section `How to manually add to an existing repository` for more info.
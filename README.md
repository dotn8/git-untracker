# git-untracker

A set of Git hooks that act like `.gitignore` for tracked files.

To be more specific: once you have this software installed, all new git clones will support the `.gituntrack` functionality. Here's what the `.gituntrack` file does:

1. If a tracked file is listed in `.gituntrack` then that file's changes will never be committed.
2. Files can be removed from `.gituntrack` and added to it; when `.gituntrack` is committed with these changes, these changes will be applied to the git repository.

## How to install

1. Download the git-untracker repository: `git clone https://github.com/JohnBillington/git-untracker.git`
2. Make the `git_template_dir` the default git template like this: `git config --global init.templateDir path/to/git-untracker/git_template_dir`
3. Any pre-existing git repositories must have `git init` executed in their root in order to add `git-untracker` functionality to them. New repositories will automatically have `git-untracker` installed in them.

### How to verify installation

1. Edit and save changes to the file `i-am-untracked.txt`
2. Try to commit *everything* either via the command line (`git commit -a`) or via your favorite git user interface. If and only if `i-am-untracked.txt` does not show up as changed, then `git-untracker` is installed successfully.

## How to add to an existing repository

1. Make sure `git-untracker` is installed
2. In the root of the repository in question, run `git init`. Running `git init` again does not erase the repository contents, it serves mostly just to update anything that changed in the git template.

## How to untrack a file

1. Make sure the file has been committed.
2. Add the file path to `.gituntrack`. The path must be relative to the root of the repository. File name patterns are not supported.
3. Commit the changes to `.gituntrack`. Be aware: the file stops being tracked *after* `.gituntrack` is committed with the file name inside it. So, if there are any uncommitted changes in the file when `.gituntrack` is being committed, `git` will try to commit the file as well. In other words, there may be one last commit to the file if you commit all your changes.

## Shortcomings

1. Right now the `.gituntrack` file must be in the repository root.

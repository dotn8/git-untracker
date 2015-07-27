# git-untracker

A set of Git hooks that act like `.gitignore` for tracked files.

To be more specific: once you have this software installed, all new git clones will support the `.gituntrack` functionality. Here's what the `.gituntrack` file does:

1. If a tracked file is listed in `.gituntrack` then that file's changes will never be committed.
2. If an untracked file is listed in `.gituntrack` then that file 

## How to install

1. Download the git-untracker repository: `git clone https://github.com/JohnBillington/git-untracker.git`
2. Make the `git_template_dir` the default git template like this: `git config --global init.templateDir path/to/git-untracker/git_template_dir`

### How to verify installation

1. In the clone of this repository, run `git init` to make sure that you're using the latest global git templates directory
2. Edit and save changes to the file i-am-untracked.txt
3. 

## How to add to an existing repository

1. Make sure `git-untracker` is installed
2. In the root of the repository in question, run `git init`. Running `git init` again does not erase the repository contents, it serves mostly just to update anything that changed in the git template.

## Shortcomings

Right now, this software assumes the following:

1. No untracked files are listed in `.gituntrack`
2. No non-existent files are listed in `.gituntrack`
3. Every line in `.gituntrack` is a valid file name

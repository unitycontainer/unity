# Contributing to Unity Container libraries

You can contribute to Unity Container with issues and PRs. Simply filing issues for problems you encounter is a great way to contribute. Contributing implementations is even better.

## The Contribution

Project maintainers will merge changes that improve the product significantly and broadly and that align with the feature roadmap.
Maintainers will not merge changes that have narrowly-defined benefits, due to compatibility risk.

The Unity Container is used by many companies and products and we reserve right to revert changes if they are found to be breaking.

Contributions must also satisfy the other published guidelines defined in this document.

## DOs and DON'Ts

Please do:

* **DO** follow generally accepted [coding styles](https://github.com/dotnet/runtime/blob/master/docs/coding-guidelines/coding-style.md) (C# code-specific)
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** include tests when adding new features. When fixing bugs, start with
  adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up
  it's often better to create new issue than to side track the discussion.

Please do not:

* **DON'T** make PRs for style changes.
* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion so we can agree on a direction before you invest a large amount
  of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to Unity Container, file an issue and start a discussion before proceeding.
* **DON'T** add API additions without filing an issue and discussing with us first. Unity team adheres to generally accepted guidelines described in [.Net Runtime API Review Process](https://github.com/dotnet/runtime/blob/master/docs/project/api-review-process.md).

## Breaking Changes

Contributions must maintain `API signature` and behavioral compatibility. Contributions that include `breaking changes` might be rejected. Please file an issue to discuss your idea or change if you believe that it may improve code.

## Suggested Workflow

We use and recommend the following workflow:

1. Create an issue for your work.
    * Reuse an existing issue on the topic, if there is one.
    * Get agreement from the team and the community that your proposed change is a good one.
    * Clearly state that you are going to take on implementing it, if that's the case. You can request that the issue be assigned to you. Note: The issue filer and the implementer don't have to be the same person.
2. Create a personal fork of the repository on GitHub (if you don't already have one).
3. In your fork, create a branch off of master (`git checkout -b mybranch`).
    * Name the branch so that it clearly communicates your intentions, such as issue-123 or githubhandle-issue.
    * Branches are useful since they isolate your changes from incoming changes from upstream. They also enable you to create multiple PRs from the same fork.
4. Make and commit your changes to your branch.
5. Add new tests corresponding to your change, if applicable.
6. Build the repository with your changes.
    * Make sure that the builds are clean.
    * Make sure that the tests are all passing, including your new tests.
7. Create a pull request (PR) against the unitycontainer/xxxx repository's **develop** branch.
    * State in the description what issue or improvement your change is addressing.
    * Check if all the Continuous Integration checks are passing.
8. Wait for feedback or approval of your changes from the owners.
9. When owners have signed off, and all checks are green, your PR will be merged.
    * The next official build will automatically include your change.
    * You can delete the branch you used for making the change.

Note: It is OK for your PR to include a large number of commits. Once your change is accepted, you will be asked to squash your commits into one or some appropriately small number of commits before your PR is merged.

Note: It is OK to create your PR as "[WIP]" on the upstream repo before the implementation is done. This can be useful if you'd like to start the feedback process concurrent with your implementation. State that this is the case in the initial PR comment.

## Commit Messages

Please format commit messages as follows (based on [A Note About Git Commit Messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)

```shell
Summarize change in 50 characters or less

Provide more detail after the first line. Leave one blank line below the
summary and wrap all lines at 72 characters or less.

If the change fixes an issue, leave another blank line after the final
paragraph and indicate which issue is fixed in the specific format
below.

Fix #42
```

## Contributor License Agreement

You must sign a [.NET Foundation Contribution License Agreement (CLA)](https://cla.dotnetfoundation.org) before your PR will be merged. This is a one-time requirement for projects in the .NET Foundation. You can read more about [Contribution License Agreements (CLA)](http://en.wikipedia.org/wiki/Contributor_License_Agreement) on Wikipedia.

The agreement: [net-foundation-contribution-license-agreement.pdf](https://github.com/dotnet/home/blob/master/guidance/net-foundation-contribution-license-agreement.pdf)

You don't have to do this up-front. You can simply clone, fork, and submit your pull-request as usual. When your pull-request is created, it is classified by a CLA bot. If the change is trivial (for example, you just fixed a typo), then the PR is labelled with `cla-not-required`. Otherwise it's classified as `cla-required`. Once you signed a CLA, the current and all future pull-requests will be labelled as `cla-signed`.

## Building and Debugging locally

To build and execute project locally please follow these steps:

* `git clone https://github.com/unitycontainer/unity.git`
* `cd unity && git checkout develop && git submodule update --init --recursive`
* open solution in Visual Studio and build

Checklist:
-----------------
- Go to lib folder. Check dll versions of all files (increase version number every new version to be released, same in all files)
- Update changelog file under src\changelog.md to match and write down whats included in this release.
- Increase version in all affected nuspec-files in this folder
- Run the publish_ACC nuget.bat files to create new nuget-packages under the publish folder. Check that the new packages have the right version.
- Fix versions in publish_to_nugetserver.bat file to right version and publish it to the nuget-server automatically (both packages).
- Commit and publish the new release to github
- Go to GitHub and create a new release/tag with same version number, copy changelog-text to tell what the release includes.



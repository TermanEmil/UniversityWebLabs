# Lab 2 - Introduction to MVC patterns

## Run instructions
This project was done using on .Net Core (with Visual Studio on macos). Open the project with Visual Studio and hit `Run`.

## Main notions
* For this project, I used a shared Layout between multiple pages. This layout contains the main navigation bar as well as the general project's styles. It also has the footer, in this way, only small portions of html must be added to each View.

* Each view has a respective Controller which simply returns the View.

* Some struggle was met with the shared navigation bar as it has to be different on each View. The problem was solved using the `ViewData` dictionary in which the title of the current page is stored.

## Conclusion
Comparing with the first lab, the MVC patterns help us keep the project more organized and intuitive so that it can be understood by everyone.

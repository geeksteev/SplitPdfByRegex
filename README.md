# SplitPdfByRegex
Parses a pdf for a regex string and creates a new pdf for each specific match.

I refer to a regular expression match as a department code as this was how I implemented the application.

Testing PDF - The pdf I was using to test was 200+ pages and contained 90 departments. Each page contained information for one department. A department can have multiple pages of information. There are no pages in
the pdf that contain information for more than one department. Each page also contained a department code in the header.

This application reads through each page of a pdf until it finds a department code. Once it finds a department code, it creates a new pdf
for that department. If it finds mulitiple pages for that department, it will append those pages to the newly created pdf respective to that department.

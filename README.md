## Calories app Repository

This project contains a mobile app (`/Calories.App`) and the corresponding server (`/server`) for the input of calories.

Please refer to each directory's readme for more info on installation and configuration of each app.


Requirements:
```
User must be able to create an account and log in. (If a mobile application, this means that more users can use the app from the same phone).

When logged in, a user can see a list of his meals, also he should be able to add, edit and delete meals. (user enters calories manually, no auto calculations!)

Implement at least three roles with different permission levels: a regular user would only be able to CRUD on their owned records, a user manager would be able to CRUD users, and an admin would be able to CRUD all records and users.

Each entry has a date, time, text, and num of calories.

Filter by dates from-to, time from-to (e.g. how much calories have I had for lunch each day in the last month if lunch is between 12 and 15h).

User setting – Expected number of calories per day.

When meals are displayed, they go green if the total for that day is less than expected number of calories per day, otherwise they go red.

REST API. Make it possible to perform all user actions via the API, including authentication (If a mobile application and you don’t know how to create your own backend you can use Firebase.com or similar services to create the API).

In any case, you should be able to explain how a REST API works and demonstrate that by creating functional tests that use the REST Layer directly. Please be prepared to use REST clients like Postman, cURL, etc. for this purpose.

Functional UI/UX design is needed. You are not required to create a unique design, however, do follow best practices to make the project as functional as possible.

Bonus: unit and e2e tests.
```
const logger = require('morgan');
const express = require('express');
const cookieParser = require('cookie-parser');

const AuthManager = require('calories/authentication/manager/authentication-manager');

const mealRoutes = require('calories/meals/rest-api/routes');
const usersRoutes = require('calories/users/rest-api/routes');
const authenticationRoutes = require('calories/authentication/rest-api/routes');

const app = express();

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());

app.use(AuthManager.createMiddleware());

app.use('/meals', mealRoutes);
app.use('/users', usersRoutes);
app.use('/authentication', authenticationRoutes);

module.exports = app;

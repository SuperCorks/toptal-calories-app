const User = require('../entities/user');
const router = require('express').Router();
const UserDao = require('../daos/user-dao');
const UserSettings = require('../entities/user-settings');

const userDao = new UserDao();

const userPermissions = require('../permissions');

// search users
router.get('/', async (req, res, next) => {
    try {

        if (!req.user) {
            res.status(403).end();
            return next();
        }

        const permission = await userPermissions.can(req.user.role)
            .context({userRolesFilter: req.query.roles})
            .execute('read').on('users');

        if (permission.granted) {

            const roles = req.query.roles !== undefined &&
                req.query.roles.split(',').map(role => role.trim().toLocaleLowerCase());

            const users = await userDao.search(roles || undefined);

            res.status(200).send(users);
        } else {
            res.status(403).end();
        }

    } catch (e) { next(e); }
});

// commit a user
router.post('/', async (req, res, next) => {
    try {

        if (!req.user) {
            res.status(403).end();
            return next();
        }

        const permission = await userPermissions.can(req.user.role)
            .context({userRole: req.body.role})
            .execute('commit').on('user');

        if (permission.granted) {

            if (!req.body) return next(new Error('no_body'));

            let committedUser = await userDao.commit(new User(req.body));

            res.status(200).send(committedUser);

        } else {
            res.status(403).end();
        }

    } catch (e) {
        return next(e);
    }
});


// commit user settings
router.post('/settings/:settingsUserId', async (req, res, next) => {
    try {

        if (!req.user) {
            res.status(403).end();
            return next();
        }

        const permission = await userPermissions.can(req.user.role)
            .context({reqUserId: req.user.id, settingsUserId: req.params.settingsUserId})
            .execute('commit').on('user-settings');

        if (permission.granted) {

            if (!req.body) return next(new Error('no_body'));

            let committedSettings = await userDao.commitSettings(req.params.settingsUserId, new UserSettings(req.body));

            res.status(200).send(committedSettings);

        } else {
            res.status(403).end();
        }

    } catch (e) {
        return next(e);
    }
});

// delete a user
router.delete('/:userId', async (req, res, next) => {
    try {

        if (!req.user) {
            res.status(403).end();
            return next();
        }

        const permission = await userPermissions.can(req.user.role)
            .context({userRole: req.body.role})
            .execute('commit').on('user');

        if (permission.granted) {

            if (req.params.userId === undefined) return next(new Error('no_user_id'));

            await userDao.delete(req.params.userId);

            res.status(200).end();

        } else {
            res.status(403).end();
        }

    } catch (e) {
        return next(e);
    }
});

module.exports = router;
const router = require('express').Router();
const AuthenticationManager = require('calories/authentication/manager/authentication-manager');

// login / signup using OAuth2
router.post('/session', async (req, res, next) => {
    try {

        if (!req.body) return next(new Error('no_body'));

        if (!req.body.rawJwt) {

            res.status(400).send({error: 'no_session_access_token_jwt'});

            return next();
        }

        let session = await AuthenticationManager.login(req.body.rawJwt);

        res.send(session);

    } catch (e) {
        next(e);
    }
});

// unsafe login (no password)
router.get('/login/unsafe/:username', async (req, res, next) => {
    try {

        if (!req.params.username) return next(new Error('invalid_param: username'));

        let session = await AuthenticationManager.loginUnsafe(req.params.username);

        res.send(session);

    } catch (e) {
        next(e);
    }
});

// unsafe signup (no password)
router.get('/signup/unsafe/:username', async (req, res, next) => {
    try {

        if (!req.params.username) return next(new Error('invalid_param: username'));

        let session = await AuthenticationManager.signupUnsafe(req.params.username);

        res.send(session);

    } catch (e) {
        next(e);
    }
});

module.exports = router;
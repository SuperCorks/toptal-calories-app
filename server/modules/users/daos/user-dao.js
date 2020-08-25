const User = require('../entities/user');
const {UserModel} = require('../storage/mongoose-user');

/** @description Data access object for {@link User} entities storage. */
class UserDao {

    /**
     * @description Retrieves a single {@link User} record from storage.
     *
     * @param {string} username - The user's unique username.
     *
     * @returns {Promise<User|null>} The user that corresponds to the username or `null` if no user matches.
     */
    async get(username) {

        let foundUser = await UserModel.findOne({username});

        if (!foundUser) return null;

        return new User(foundUser);
    }

    /**
     * @description Retrieves {@link User} records from storage.
     *
     * @param {UserRoles[]|undefined} [roles] - The roles of the users to return.
     *
     * @returns {Promise<User[]>}
     */
    async search(roles) {

        let filters = {};

        if (roles) filters = {role: {$in: roles}};

        return (await UserModel.find(filters)).map(doc => new User(doc));
    }

    /**
     * @description Creates or updates a user record in storage.
     *
     * @param {User} user - The user to commit to storage.
     *
     * @return {Promise<User>} A promise that resolves when the operation finishes and returns
     * the committed user.
     */
    async commit(user) {

        if (!user) throw new Error('null_param: user');

        if (user.id) {

            let userDoc = await UserModel.findOne({_id: user.id});

            userDoc = await userDoc.overwrite(user).save();

            return new User(userDoc);

        } else {

            if (user.role === 'member' && !user.settings) user.settings = {dailyCalories: 1000};

            let userDoc = await new UserModel(user).save();

            return new User(userDoc);
        }
    }

    /**
     * @description Updates the {@link UserSettings} of the user that corresponds to the given id.
     *
     * @param {string} userId - The id of the user who owns the settings.
     *
     * @param {UserSettings} newSettings - The new settings for that user.
     *
     * @returns {Promise<UserSettings>} A promise that resolves when the operation finishes and returns the committed
     * settings.
     */
    async commitSettings(userId, newSettings) {

        let user = await UserModel.findOne({_id: userId});

        user.set({settings: newSettings});

        await user.save();

        return newSettings;
    }

    /**
     * @description Deletes the record associated to the user's id.
     *
     * @param {string} userId - The id of user to delete.
     *
     * @return {Promise} A promise that resolves when the operation completes.
     */
    delete(userId) {

        if (!userId) throw new Error('null_param: userId');

        return UserModel.deleteOne({_id: userId});
    }
}

module.exports = UserDao;
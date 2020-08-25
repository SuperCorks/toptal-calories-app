const {AccessControl} = require('role-acl');
const ac = new AccessControl();

// ADMIN PERMISSIONS
// Admins can CRUD all records
ac.grant('admin').execute('*').on('users');
ac.grant('admin').execute('*').on('user');
ac.grant('admin').execute('*').on('user-settings');

// MANAGER PERMISSIONS
// Managers can CRUD members (only members).
ac.grant('manager').condition(context => {

    if (!context || !Array.isArray(context.rolesFilter)) return false;

    return context.rolesFilter.length === 1 && context.rolesFilter[0] === 'member';

}).execute('read').on('users');

for (let action of ['commit', 'delete']) {

    ac.grant('manager').condition(context => {

        if (!context || !context.userRole) return false;

        return context.userRole === 'member';

    }).execute(action).on('user');
}

ac.grant('manager').condition({Fn: 'EQUALS', args: {userRole: 'member'}}).execute('commit').on('user-settings');


// MEMBER PERMISSIONS
// Members can commit their own settings
ac.grant('member').condition({Fn: 'EQUALS', args: {reqUserId: '$.settingsUserId'}}).execute('commit').on('user-settings');

module.exports = ac;
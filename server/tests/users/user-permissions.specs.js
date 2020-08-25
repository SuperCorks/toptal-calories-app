const {expect, fail, assert} = require('test-utils/functions');

const userPermissions = require('calories/users/permissions');

describe('calories/users/permissions', () => {

    context('read:users', () => {

        it('admins should be able to CRUD all users', async () => {

            assert((await userPermissions.can('admin').execute('read').on('users')).granted, `Admin can't read users!`);
        });

        it('managers should be able to CRUD members', async () => {

            expect(
                (await userPermissions.can('manager').execute('read').on('users')).granted === false,
                `Managers can read all users but shouldn't!`
            );

            for (let invalidRolesFilter of [
                [],
                ['admin'],
                ['manager'],
                ['admin', 'manager'],
                ['manager', 'admin'],
                ['member', 'manager'],
                ['member', 'admin'],
                ['member', 'manager', 'admin']
            ]){
                let context = {rolesFilter: invalidRolesFilter};
                assert(
                    (await userPermissions.can('manager').context(context).execute('read').on('users')).granted === false,
                    `Managers should'nt be able to read:users when context is: ${JSON.stringify(context)} !`
                );
            }

            assert(
                (await userPermissions.can('manager').context({rolesFilter: ['member']}).execute('read').on('users')).granted,
                `Managers should be able to read:users when context is: ${JSON.stringify({rolesFilter: ['member']})} but can't!`
            );
        });

        it(`members should'nt be able to read:users`, async () => {

            assert(
                (await userPermissions.can('member').execute('read').on('users')).granted === false,
                `Members can read users!`
            );
        });
    });

    context('commit:user', () => {

        it('admins should be able to CRUD all users', async () => {

            assert((await userPermissions.can('admin').execute('commit').on('user')).granted, `Admin can't commit:user!`);
        });

        it('managers should be able to CRUD members', async () => {

            expect(
                (await userPermissions.can('manager').execute('commit').on('user')).granted === false,
                `Managers can commit all users but shouldn't!`
            );

            for (let invalidContext of [
                {userRole: 'manager'},
                {userRole: 'admin'},
            ]){

                assert(
                    (await userPermissions.can('manager').context(invalidContext).execute('commit').on('user')).granted === false,
                    `Managers should'nt be able to commit:user when context is: ${JSON.stringify(invalidContext)} !`
                );
            }

            assert(
                (await userPermissions.can('manager').context({userRole: 'member'}).execute('commit').on('user')).granted,
                `Managers should be able to commit:users when context is: ${JSON.stringify({userRole: 'member'})} but can't!`
            );
        });

        it(`members should'nt be able to commit:user`, async () => {

            const unauthorizedContexts = [
                undefined,
                {userRole: 'member'},
                {userId: 'a1', reqUserId: 'a1'}
            ];

            for (let context of unauthorizedContexts){

                assert(
                    (await userPermissions.can('member').context(context).execute('commit').on('user')).granted === false,
                    `Members shouldn't be able to commit:user in any context !`
                );
            }
        });
    });

    context('delete:user', () => {

        it('admins should be able to CRUD all users', async () => {

            assert((await userPermissions.can('admin').execute('delete').on('user')).granted, `Admin can't delete:user!`);
        });

        it('managers should be able to CRUD members', async () => {

            expect(
                (await userPermissions.can('manager').execute('delete').on('user')).granted === false,
                `Managers can delete all users but shouldn't!`
            );

            for (let invalidContext of [
                {userRole: 'manager'},
                {userRole: 'admin'},
            ]){

                assert(
                    (await userPermissions.can('manager').context(invalidContext).execute('delete').on('user')).granted === false,
                    `Managers should'nt be able to delete:user when context is: ${JSON.stringify(invalidContext)} !`
                );
            }

            assert(
                (await userPermissions.can('manager').context({userRole: 'member'}).execute('delete').on('user')).granted,
                `Managers should be able to delete:users when context is: ${JSON.stringify({userRole: 'member'})} but can't!`
            );
        });

        it(`members should'nt be able to delete:user`, async () => {

            const unauthorizedContexts = [
                undefined,
                {userRole: 'member'},
                {userId: 'a1', reqUserId: 'a1'}
            ];

            for (let context of unauthorizedContexts){

                assert(
                    (await userPermissions.can('member').context(context).execute('delete').on('user')).granted === false,
                    `Members shouldn't be able to delete:user in any context !`
                );
            }
        });
    });

    context('commit:user-settings', () => {

        it('admins should be able to CRUD all users', async () => {

            assert(
                (await userPermissions.can('admin').execute('commit').on('user-settings')).granted,
                `Admin can't commit:user-settings!`
            );
        });

        it('managers should be able to CRUD members', async () => {

            expect(
                (await userPermissions.can('manager').execute('commit').on('user-settings')).granted === false,
                `Managers can commit all user-settings but shouldn't!`
            );

            for (let invalidContext of [
                {userRole: 'manager'},
                {userRole: 'admin'},
            ]){

                assert(
                    (await userPermissions.can('manager').context(invalidContext).execute('commit').on('user-settings')).granted === false,
                    `Managers should'nt be able to commit:user-settings when context is: ${JSON.stringify(invalidContext)} !`
                );
            }

            assert(
                (await userPermissions.can('manager').context({userRole: 'member'}).execute('commit').on('user-settings')).granted,
                `Managers should be able to commit:user-settings when context is: ${JSON.stringify({userRole: 'member'})} but can't!`
            );
        });

        it(`members should be able to commit their own user settings`, async () => {

            const unauthorizedContexts = [
                undefined,
                {userRole: 'member'},
                {userId: 'b1', reqUserId: 'a1'}
            ];

            for (let context of unauthorizedContexts){

                assert(
                    (await userPermissions.can('member').context(context).execute('commit').on('user-settings')).granted === false,
                    `Members shouldn't be able to commit:user-settings when context is : ${unauthorizedContexts} !`
                );
            }

            let validContext = {settingsUserId: 'a1', reqUserId: 'a1'};
            assert(
                (await userPermissions.can('member').context(validContext).execute('commit').on('user-settings')).granted,
                `Members should be able to commit:user-settings when context is: ${JSON.stringify(validContext)} but can't!`
            );
        });
    });
});
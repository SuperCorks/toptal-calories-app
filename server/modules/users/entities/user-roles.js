

/**
 * @enum
 * @description The standard user role, it can CRUD its own records.
 */
const UserRoles = {
    /** @description The administrator user role, it can CRUD all records ands users. */
    Admin: 'admin',
    /** @description The manager user role, it can CRUD member user records (the user only). */
    Manager: 'manager',
    /** @description The standard user role, it can CRUD its own records. */
    Member: 'member'
};

module.exports = UserRoles;

const DefaultConfigs = {host: 'localhost', database: 'calories'};
const ProcessConfigs = Object.assign({}, DefaultConfigs);

const ProcessArgs = {
    host:     { arg: '--mongo-host' },
    database: { arg: '--mongo-db'   },
};

// Extracts the host and database mongo configs from process arguments.
for(let mongoConfigName in ProcessArgs){

    let argIndex = process.argv.indexOf(ProcessArgs[mongoConfigName].arg);

    if (argIndex !== -1 && process.argv[argIndex + 1] != null){

        ProcessConfigs[mongoConfigName] = process.argv[argIndex + 1];
    }
}

/**
 * TODO
 */
class MongoConfigs {

    constructor() {

        /**
         * @description The current database configuration.
         *
         * @type {{database: string, host: string}}
         */
        this.current = this.process;
    }

    /**
     * @description Resets the {@link current} configs to the process configs.
     */
    reset() { this.current = this.process; }

    /**
     * @description Returns a copy of the configs passed to the node process at start. If no argument was passed to
     * the process, this is equal to the {@link default} configs.
     *
     * @return {{database: string, host: string}}
     */
    get process() { return Object.assign({}, ProcessConfigs); }

    /**
     * @description Returns a copy of the default configs.
     *
     * @return {{database: string, host: string}}
     */
    get default() { return Object.assign({}, DefaultConfigs); }
}

module.exports = new MongoConfigs(); // singleton

// TYPE DEFS

/**
 * @typedef {object} MongoParams
 * @property {string} host - The mongodb host (e.g. 'localhost:27017' or 'example.com').
 * @property {string} database - The database's name (e.g. 'customers')
 */
## Calories app - NodeJs Server

#### Dependencies
*  MongoDb v4.2.3 ([Download here](https://www.mongodb.com/download-center/community))
*  NodeJs v12.13.1 ([Download here](https://nodejs.org/en/download/))

#### Installation

```bash
# from /server directory
npm install
npm run seed -- --mongo-host localhost --mongo-db calories
```

#### Start Server

```bash
# from /server directory
npm start -- --mongo-host localhost --mongo-db calories
```

#### Tests

```bash
# from /server directory
npm test
```

Checkout `/server/bin/rest-api.http` for example http requests on the exposed api.

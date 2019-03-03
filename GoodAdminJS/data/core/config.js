//Constants
const json = require("json-file");
const fs = require("fs");

module.exports.load = () => {
    return new Promise((Resolve, Reject) => {
        fs.readFile("./data/core/config.json", 'utf8', function (err, data) {
            if (err) { Reject( "reeeee something no work. ERROR:" + err ) }
            
            Resolve(data);

        })
    })
}
//Documentation for Discord.js
//   https://discord.js.org


//Constants
const discord = require("Discord.js");
const json = require("json-file");
const fs = require("fs");
const client = new discord.Client();

//Modules
const config = require("./data/core/config.js").load().then(data => {
    //const _CONFIG = JSON.parse(data.substring(1));
    client.login(_CONFIG.TOKEN);
}).catch(err => {
    console.log("err occured : " + err);
 });


//Events
var events = ["guildCreate", "guildRemove", "channelCreate", "channelDelete", "channelUpdate", "error", "guildMemberAdd", "message", "messageDelete", "roleCreate", "roleDelete", "roleUpdate", "warn", "userUpdate"];
for (i = 0; i < events.length; i++) {
    if (fs.existsSync("./data/core/events/" + events[i] + ".js")) {
        client.on(events[i], function (a, b, c, d, e, f, g) {
            try {
                require("./data/core/events/" + events[i] + ".js").execute(a, b, c, d, e, f, g);
            } catch(e) {
                sendError(e);
            }
        });
    }
}

//Functions
function sendError(e) {
    
}

client.on("ready", function () { console.log("Started.") })

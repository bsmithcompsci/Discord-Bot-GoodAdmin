//Documentation for Discord.js
//   https://discord.js.org

//Constants
const discord = require("Discord.js");
const json = require("json-file");
const fs = require("fs");
const client = new discord.Client();
process.env.client = client;

const config = require("./data/core/config.js").load().then(data => {
    const _CONFIG = JSON.parse(data.substring(1));
    process.env.CONFIG = _CONFIG;
    client.login(_CONFIG.TOKEN);
}).catch(err => {
    console.log("err occured : " + err);
    });

//One Line Events
client.on("ready", function () { require("./data/core/events/ready.js").execute(client) });

//Events
var events = ["guildCreate", "guildRemove", "channelCreate", "channelDelete", "channelUpdate", "error", "guildMemberAdd", "message", "messageDelete", "roleCreate", "roleDelete", "roleUpdate", "warn", "userUpdate"];
for (i = 0; i < events.length; i++) {
    if (fs.existsSync("./data/core/events/" + events[i] + ".js")) {
        if (!events[i])
            continue;

        const _LEVENT = events[i];
        console.log("Loading " + events[i] + "...");
        client.on(events[i], function (a, b, c, d, e, f, g) {
            var ev = _LEVENT;
            try {
                require("./data/core/events/" + ev + ".js").execute(client, a, b, c, d, e, f, g);
            } catch (e) {
                console.log("Error occured: "+e)
            }
        });
        console.log("Loaded " + events[i]);
    }
}
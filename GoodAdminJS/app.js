//Documentation for Discord.js
//   https://discord.js.org

//Constants
const discord = require("Discord.js");
const json = require("json-file");
const fs = require("fs");
const client = new discord.Client();
let CMDS;
process.env.client = client;

const config = require("./data/core/config.js").load().then(data => {
    const _CONFIG = JSON.parse(data.substring(1));
    process.env.CONFIG = _CONFIG;
    client.login(_CONFIG.TOKEN);
}).catch(err => {
    //Log for debug :("err occured : " + err);
    });

fs.readdir("./commands/", function (err, data) {
    CMDS = data;
    for (i in data) { CMDS[i] = { data: require("./commands/" + data[i]), cmd: data[i] } }
});

//One Line Events
client.on("ready", function () { require("./data/core/events/ready.js").execute(client) });
client.on("message", function (o) {
    if (!o.author.bot) {
        try {
            var e = o.content.split(" ").splice(1).join(" "),
                r = o.content.split("!")[1].split(" ")[0];
            var c = require("./commands/" + r + ".js");
            var res = c.execute(client, o, e, { cmds: CMDS });
        } catch(e) {

        }
    }
});

//Events
var events = ["guildCreate", "guildRemove", "channelCreate", "channelDelete", "channelUpdate", "error", "guildMemberAdd", "messageDelete", "roleCreate", "roleDelete", "roleUpdate", "warn", "userUpdate"];
for (i = 0; i < events.length; i++) {
    if (fs.existsSync("./data/core/events/" + events[i] + ".js")) {
        if (!events[i])
            continue;

        const _LEVENT = events[i];
        //Log for debug :("Loading " + events[i] + "...");
        client.on(events[i], function (a, b, c, d, e, f, g) {
            var ev = _LEVENT;
            try {
                var otherdata = { cmds: CMDS };
                require("./data/core/events/" + ev + ".js").execute(client, otherdata, a, b, c, d, e, f, g);
            } catch (e) {
                //Log for debug :("Error occured: "+e)
            }
        });
        //Log for debug :("Loaded " + events[i]);
    }
}
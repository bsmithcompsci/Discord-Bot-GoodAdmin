const disc = require("discord.js");
const fs = require("fs");

module.exports.execute = (client, odata, msg) => {
    if (msg.author.bot) { return; };
    var args = msg.content.split(" ").splice(1).join(" ");
    var cmd = msg.content.split("!")[1].split(" ")[0];

    fs.readdir("./commands/", function (err, data) {
        for (i in data) {
            //Log for debug :("|" + data[i] + "|");
            //Log for debug :("|" + cmd + ".js|");
            if (data[i] == cmd) {
                //Log for debug :("Works");
                try {
                    var e = require("./commands/" + cmd);
                    var er = e.execute(client, msg, args, odata);
                    return {
                        ERROR: false,
                        RET: er
                    }
                } catch (e) {
                    //Log for debug :("err "+e)
                    return {
                        ERROR: true,
                        ErrorMSG: e,
                        date: new Date(),
                        args: { cmd: cmd, args: args, author: msg.author.tag }
                    }
                }
            }
        }
    })
}
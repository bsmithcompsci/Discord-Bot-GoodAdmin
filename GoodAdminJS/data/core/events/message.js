const disc = require("discord.js");
const fs = require("fs");

module.exports.execute = (client, msg) => {
    if (msg.author.bot) { return; };
    var args = msg.content.split(" ").splice(1).join(" ");
    var cmd = msg.content.split("!")[1].split(" ")[0];

    fs.readdir("./commands/", function (err, data) {
        for (i in data) {
            console.log(data[i]);
        }
    })

    if (!fs.exists("./commands/"))
        return console.log("not existant");

    try {
        var e = require("./commands/" + cmd);
        if (!config) { var er = e.execute(client, msg, args); } else { var er = e.execute(client, msg, args, config); }
        return {
            ERROR: false,
            RET: er
        }
    } catch (e) {
        return {
            ERROR: true,
            ErrorMSG: e,
            date: new Date(),
            args: {cmd:cmd,args:args,author:msg.author.tag}
        }
    }
}
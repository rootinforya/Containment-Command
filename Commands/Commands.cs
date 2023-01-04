using CommandSystem;
using GameCore;
using PlayerRoles;
using PluginAPI.Core;
using System;
using static MapGeneration.ImageGenerator;
using System.Runtime.Remoting.Contexts;
using MapGeneration;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using PluginAPI.Core.Doors;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Zones;


namespace Containment.Command
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Containment : ICommand
    {
        public string Command { get; } = "conf";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "If you're an SCP you can commit suicide with this command. Do it :)";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(((CommandSender)sender).SenderId);
            if (player.Role.GetTeam() != Team.SCPs) { response = "You are not an SCP"; return false; }
            else if (Player.GetPlayers().FirstOrDefault(p => p.Role == RoleTypeId.Scp079) != null)
            {
                response = "SCP 079 can help you!";
                return false;
            }
            else if (player.Role.GetTeam() == Team.SCPs)
            {
                var room = RoomIdUtils.RoomAtPosition(player.GameObject.transform.position);
                if (room.Name == RoomName.Hcz096 || room.Name == RoomName.HczArmory || room.Name == RoomName.HczMicroHID || room.Name == RoomName.LczArmory)
                {
                    foreach (var door in DoorVariant.DoorsByRoom[room].Where(door => IsItAnAllowedRoom(door))) // This is IMPORTANT
                    {
                        door.NetworkTargetState = false;
                        door.ServerChangeLock(DoorLockReason.Isolation, true);
                    }

                    player.Kill("Recontained");
                    response = "You've been contained";
                    return true;
                }
                else if (room.Name == RoomName.Hcz049)
                {
                    if (player.Role == RoleTypeId.Scp049)
                        foreach (var door in DoorVariant.DoorsByRoom[room].Where(door => !IsItAnAllowedRoom(door)))
                        {

                            door.NetworkTargetState = false;
                            door.ServerChangeLock(DoorLockReason.Isolation, true);
                            break;
                        }
                    else
                    {
                        foreach (var door in DoorVariant.DoorsByRoom[room].Where(door => IsItAnAllowedRoom(door)))
                        {
                            door.NetworkTargetState = false;
                            door.ServerChangeLock(DoorLockReason.Isolation, true);
                        }
                    }
                    player.Kill("Recontained");
                    response = "You've been contained";
                    return true;
                }
                else
                {
                    response = "You cannot be contained here!";
                    return false;
                }
            }
            else { response = "ERROR"; return false; }
        }
        private static bool IsItAnAllowedRoom(DoorVariant doorVariant) // This is IMPORTANT
        {
            var nameTag = doorVariant.TryGetComponent(out DoorNametagExtension name) ? name.GetName : null;
            if (nameTag == null) return false;
            var bracketStart = nameTag.IndexOf('(') - 1;
            if (bracketStart > 0)
                nameTag = nameTag.Remove(bracketStart, nameTag.Length - bracketStart);
            return nameTag == "096" || nameTag == "HCZ_ARMORY" || nameTag == "HID" || nameTag == "LCZ_ARMORY" || nameTag == "049_ARMORY";
        }
    }
}

# Less Zdo Corruption

This is a mod that contains a collection of various fixes attempted to resolve Valheims issue with save-data.

Valheim recently started having a specific limit of how many data entries can be in each entity that is persisted or syncronized. This results in issues due to the code not taking into account that modded games might cause the vanilla code to start storing a lot more information than expected.

An example is how each zone keeps track of when a creature could/should spawn. This list can grow a lot bigger if using mods that change spawning or raids. If the list grows big enough, unexpected errors are likely to show, and the only real solution is probably to find a tool that will clear corrupted ZDO's, basically deleting the stored data. The later can be less horrible than it sounds, but its a big hazzle.

All the fixes included in this mod are intended to be standalone and can be toggled in the configurations (restart required), so if anything ends up conflicting with a mod or the fix stops working, it can be disabled.

# Fixes

## SpawnSystem - Less Records

Each zone stores a timestamp for each creature that can spawn naturally, as well as for each raid-creature. The only filter is creatures that are not set to spawn in that biome.
This solution moves the timestamp storage to happen later in the code, so that more checks can happen before (like environmental conditions, or additionally modded ones like from Spawn That).

This doesn't necessarily fix the issue with too many records, it is only intended to mitigate the problem.
# Less Zdo Corruption

A collection of various fixes and mitigations attempting to resolve Valheims issues with save-data.

Valheim recently started having a specific limit of how many data entries can be in each entity that is persisted or syncronized. This results in issues due to the code not taking into account that modded games might cause the vanilla code to start storing a lot more information than expected.

An example is how each zone keeps track of when a creature could/should spawn. This list can grow a lot bigger if using mods that change spawning or raids. If the list grows big enough, unexpected errors are likely to show, and the only real solution is probably to find a tool that will clear corrupted ZDO's, basically deleting the stored data. The later can be less horrible than it sounds, but its a hazzle.

All the fixes included in this mod are intended to be standalone and can be toggled in the configurations (restart required), so if anything ends up conflicting with a mod, or performance is affected, it can be disabled.

The mod is intended to run client-side.

# Fixes

## SpawnSystem - Less Records

Each zone stores a timestamp for each creature that can spawn naturally, as well as for each raid-creature. The only filter is creatures that are not set to spawn in that biome.
This solution moves the timestamp storage to happen later in the code, so that more checks can happen before (like environmental conditions, or additionally modded ones like from Spawn That).

This doesn't necessarily fix all scenarios with too many records, but it should help mitigate the problem.

## RandEventSystem - Cleanup

When any event (eg., a raid or a boss fight) is run, the SpawnSystem is used for actually spawning in most creatures. This means it will slowly pollute the local zone storage with more and more timestamps about what spawned when.

This fix makes the current zone-owner run a cleanup when the event ends, removing the entries of things that spawned for that event.

This is probably the most likely cause of zones getting corrupted when a lot of raids is added and run in the same area.

## Zdo - Block Overflow

Toggle for simply blocking attempts at adding more than the max limit of data to ZDO's.

Say for instance someone added 400 different creatures to spawn in meadows, that would for most of the code-base look completely valid. But if this fix is enabled, whenever the spawn system tried to store info about when it tried to spawn these in, that info simply gets thrown away and an error will be logged.

This is not exactly a wonderful solution, since not storing data means unknown side-effects, but it should still be preferable to a corrupted save.

## ZNetScene.RemoveObjects

Sometimes the game finds an object it wants to remove from its active "memory", often due to the player moving away from it, but that object is misconfigured, missing a resource or something similar.
When this problem occurs, it shows itself by endlessly spamming the log with errors about "ZNetScene.RemoveObjects".

This fix attempts to handle the problem as it occurs, and clean up the offending object so that the game doesn't keep trying and failing to do so itself. It will by default also try to log information about the problem itself, although this can be disabled in the config.

Currently multiple other mod-makers have made similar fixes for this problem, so if you have a preference, simply disable this one.
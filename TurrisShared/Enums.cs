// Events, Spawn, Despawn, GameInfo are reliable-ordered; Snapshots are sequenced
public enum PacketType : byte
{
    // Sent by a client server saying to a client to spawn their player in and weather or not they are the leader
    // Received by one client
    GameInfo,
    // Sent by a client server saying any new entity spawned in
    // Received by everyone but sender, true clients only
    SpawnEntity,
    // Sent by a client server saying any entity despawned
    // Received by everyone but sender, true clients only
    DespawnEntity,
    // Sent by a client server or client
    // if (client server) Send a snapshot for all non other player entities
    // else Send a snapshot for themselves, their player only
    // Received by everyone but sender
    EntitySnapshot, // IS A TRUE CLIENT INPUT
                    // Sent by a player saying they did something
                    // Received by everyone but sender
    EntityEventOut, // IS A TRUE CLIENT INPUT
                    // Sent by client server giving the results of something that happened in the game
                    // Received by everyone but sender, true clients only
    EntityEventIn,
    // Sent by client server saying something happened with a tile in the game
    // Received by everyone but sender, true clients only
    TileEventIn,
}
public enum PacketRoutingType : byte
{
    BroadcastButToSender = 0,
    SendToClientServer = 1,
    SendToClient = 2,
}
public enum GameInfoType : byte
{
    SelfJoin,
    Join,
    Leave
}

import { Client } from 'discord.js';
import { readyEvent } from '../events/ready.js';
import { interactionCreateEvent } from '../events/interactionCreate.js';
import { messageCreateEvent } from '../events/messageCreate.js';
import { guildCreateEvent } from '../events/guildCreate.js';

export const registerEvents = async (client: Client) => {
  client.once(readyEvent.name as string, (clientArg: any) => readyEvent.execute(clientArg));
  client.on(interactionCreateEvent.name as string, (interaction: any) => interactionCreateEvent.execute(interaction));
  client.on(messageCreateEvent.name as string, (message: any) => messageCreateEvent.execute(message));
  client.on(guildCreateEvent.name as string, (guild: any) => guildCreateEvent.execute(guild));
};

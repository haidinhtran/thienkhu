import { Client } from 'discord.js';
import { readyEvent } from '../events/ready.js';
import { interactionCreateEvent } from '../events/interactionCreate.js';

export const registerEvents = async (client: Client) => {
  client.once(readyEvent.name as string, (clientArg: any) => readyEvent.execute(clientArg));
  client.on(interactionCreateEvent.name as string, (interaction: any) => interactionCreateEvent.execute(interaction));
};

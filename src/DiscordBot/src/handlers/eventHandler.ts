import { Client } from 'discord.js';
import { readyEvent } from '../events/ready.js';
import { interactionCreateEvent } from '../events/interactionCreate.js';

export const registerEvents = async (client: Client) => {
  client.once(readyEvent.name, (...args) => readyEvent.execute(...args));
  client.on(interactionCreateEvent.name, (...args) => interactionCreateEvent.execute(...args));
};

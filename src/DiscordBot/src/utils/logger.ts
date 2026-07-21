import winston from 'winston';
import { env } from '../config/env.js';

const { combine, timestamp, printf, colorize } = winston.format;

const logFormat = printf(({ level, message, timestamp, traceId, ...meta }) => {
  // Simple masking for token just in case
  const safeMessage = typeof message === 'string' ? message.replace(env.DISCORD_TOKEN, '***') : message;
  const trace = traceId ? ` [TraceId: ${traceId}]` : '';
  const metaString = Object.keys(meta).length ? `\n${JSON.stringify(meta, null, 2)}` : '';
  return `${timestamp} ${level}: ${safeMessage}${trace}${metaString}`;
});

export const logger = winston.createLogger({
  level: env.LOG_LEVEL,
  format: combine(
    timestamp(),
    logFormat
  ),
  transports: [
    new winston.transports.Console({
      format: combine(
        colorize(),
        timestamp(),
        logFormat
      )
    })
  ]
});

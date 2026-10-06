import createClient, { type Middleware } from 'openapi-fetch';
import type { paths } from './schema';

// One id per call so API logs and audit rows can be tied back to the browser request.
const correlationId: Middleware = {
  onRequest({ request }) {
    request.headers.set('X-Correlation-ID', crypto.randomUUID());
    return request;
  },
};

export function createApiClient(baseUrl: string) {
  const client = createClient<paths>({ baseUrl });
  client.use(correlationId);
  return client;
}

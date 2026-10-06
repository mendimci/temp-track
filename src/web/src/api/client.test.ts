import { createApiClient } from './client';

describe('createApiClient', () => {
  it('sends an X-Correlation-ID header on every request', async () => {
    const fetchMock = vi.fn(async (req: Request) => {
      void req;
      return new Response(JSON.stringify({ status: 'ok' }), {
        headers: { 'content-type': 'application/json' },
      });
    });
    const api = createApiClient('http://api.test');

    await api.GET('/health', { fetch: fetchMock });

    const sent = fetchMock.mock.calls[0][0];
    expect(sent.headers.get('X-Correlation-ID')).toMatch(/^[0-9a-f-]{36}$/);
  });
});

// Placeholder until docs/api/openapi.yaml exists (TT-24); replace by running `npm run gen:api`.
export interface paths {
  '/health': {
    get: {
      responses: {
        200: { content: { 'application/json': { status: string } } };
      };
    };
  };
}

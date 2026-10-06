import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { AppShell } from './components/layout/AppShell';

const queryClient = new QueryClient();

const router = createBrowserRouter([
  {
    element: <AppShell />,
    children: [{ path: '/', element: <h1>My requests</h1> }],
  },
]);

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  );
}

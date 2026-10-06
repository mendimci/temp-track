import { render, screen } from '@testing-library/react';
import { createMemoryRouter, RouterProvider } from 'react-router-dom';
import { AppShell } from './AppShell';

function renderShell() {
  const router = createMemoryRouter(
    [
      {
        element: <AppShell />,
        children: [{ path: '/', element: <h1>Home</h1> }],
      },
    ],
    { initialEntries: ['/'] },
  );
  render(<RouterProvider router={router} />);
}

describe('AppShell', () => {
  it('mounts with header, labelled nav and main landmarks', () => {
    renderShell();

    expect(screen.getByRole('banner')).toBeInTheDocument();
    expect(
      screen.getByRole('navigation', { name: 'Primary' }),
    ).toBeInTheDocument();
    expect(screen.getByRole('main')).toHaveTextContent('Home');
  });

  it('offers a skip link targeting main', () => {
    renderShell();

    expect(
      screen.getByRole('link', { name: 'Skip to main content' }),
    ).toHaveAttribute('href', '#main');
    expect(screen.getByRole('main')).toHaveAttribute('id', 'main');
  });
});

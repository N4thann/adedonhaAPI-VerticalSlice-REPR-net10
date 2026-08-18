import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import { ProtectedRoute } from './ProtectedRoute';
import { useAuth } from '../contexts/AuthContext';

vi.mock('../contexts/AuthContext', () => ({
  useAuth: vi.fn(),
}));

function renderWithRoute(initialPath: string) {
  return render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Routes>
        <Route path="/login" element={<div>Tela de login</div>} />
        <Route element={<ProtectedRoute />}>
          <Route path="/admin/categorias" element={<div>Categorias</div>} />
        </Route>
      </Routes>
    </MemoryRouter>
  );
}

describe('ProtectedRoute', () => {
  it('redireciona para /login quando não autenticado', () => {
    vi.mocked(useAuth).mockReturnValue({ isAuthenticated: false } as ReturnType<typeof useAuth>);
    renderWithRoute('/admin/categorias');
    expect(screen.getByText('Tela de login')).toBeInTheDocument();
  });

  it('renderiza a rota filha quando autenticado', () => {
    vi.mocked(useAuth).mockReturnValue({ isAuthenticated: true } as ReturnType<typeof useAuth>);
    renderWithRoute('/admin/categorias');
    expect(screen.getByText('Categorias')).toBeInTheDocument();
  });
});

import { Navigate, Route, Routes } from 'react-router-dom';
import { PublicLayout } from '../components/templates/PublicLayout';
import { CategoriesMuralPage } from '../pages/CategoriesMuralPage';
import { CategoryWordsPage } from '../pages/CategoryWordsPage';
import { LoginPage } from '../pages/auth/LoginPage';
import { ProtectedRoute } from './ProtectedRoute';
import { AdminRoute } from './AdminRoute';
import { AdminLayout } from '../components/templates/AdminLayout';
import { DashboardPage } from '../pages/admin/dashboard/DashboardPage';
import { CategoriesPage } from '../pages/admin/categories/CategoriesPage';
import { WordsPage } from '../pages/admin/words/WordsPage';

export const AppRoutes = () => {
  return (
    <Routes>
      <Route element={<PublicLayout />}>
        <Route path="/" element={<CategoriesMuralPage />} />
        <Route path="/categorias/:categorySlug/:letra" element={<CategoryWordsPage />} />
      </Route>

      <Route path="/login" element={<LoginPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<AdminRoute />}>
          <Route element={<AdminLayout />}>
            <Route path="/admin/dashboard" element={<DashboardPage />} />
            <Route path="/admin/categorias" element={<CategoriesPage />} />
            <Route path="/admin/palavras" element={<WordsPage />} />
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default AppRoutes;

import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export const AdminRoute = () => {
  const { user } = useAuth();
  return user?.isAdmin ? <Outlet /> : <Navigate to="/login" replace />;
};

import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import type { LoginRequest } from '../types/auth.types';
import { authService } from '../services/authService';
import { decodeAuthUser, type AuthUser } from '../utils/jwt';
import { TOKEN_STORAGE_KEY } from '../services/api';

interface AuthContextType {
  user: AuthUser | null;
  isAuthenticated: boolean;
  login: (data: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem(TOKEN_STORAGE_KEY);
    if (token) {
      try {
        setUser(decodeAuthUser(token));
      } catch {
        localStorage.removeItem(TOKEN_STORAGE_KEY);
      }
    }
    setLoading(false);
  }, []);

  const login = async (data: LoginRequest) => {
    const response = await authService.login(data);
    localStorage.setItem(TOKEN_STORAGE_KEY, response.token);
    setUser(decodeAuthUser(response.token));
  };

  const logout = () => {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, login, logout }}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components -- hook colocated with its Provider, standard Context pattern
export const useAuth = () => useContext(AuthContext);

import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { Alert, Box, Button, CircularProgress, Paper, TextField, Typography } from '@mui/material';
import { useAuth } from '../../contexts/AuthContext';

export const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);
    setIsLoading(true);
    try {
      await login({ email, password });
      navigate('/admin/dashboard');
    } catch {
      setError('E-mail ou senha incorretos.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', p: 2 }}>
      <Paper elevation={3} sx={{ p: 4, maxWidth: 400, width: '100%' }}>
        <Typography variant="h5" component="h1" sx={{ mb: 3, fontWeight: 'bold' }}>
          Login Admin
        </Typography>
        <Box component="form" onSubmit={handleSubmit}>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <TextField
            label="E-mail" type="email" fullWidth required sx={{ mb: 2 }}
            value={email} onChange={(e) => setEmail(e.target.value)}
          />
          <TextField
            label="Senha" type="password" fullWidth required sx={{ mb: 3 }}
            value={password} onChange={(e) => setPassword(e.target.value)}
          />
          <Button
            type="submit" variant="contained" fullWidth disabled={isLoading}
            startIcon={isLoading ? <CircularProgress size={20} color="inherit" /> : undefined}
          >
            {isLoading ? 'Entrando...' : 'Entrar'}
          </Button>
        </Box>
      </Paper>
    </Box>
  );
};

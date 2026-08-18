import { Outlet, useNavigate, NavLink } from 'react-router-dom';
import { AppBar, Box, Button, Drawer, List, ListItemButton, ListItemText, Toolbar, Typography } from '@mui/material';
import { alpha } from '@mui/material/styles';
import { useAuth } from '../../contexts/AuthContext';

const DRAWER_WIDTH = 220;

export const AdminLayout = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1, bgcolor: 'chrome.main', color: 'chrome.contrastText' }}>
        <Toolbar sx={{ justifyContent: 'space-between' }}>
          <Typography variant="h6" component="div">AdedonhaAPI — Admin</Typography>
          <Button color="inherit" onClick={handleLogout}>Sair</Button>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="permanent"
        sx={{
          width: DRAWER_WIDTH, flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: DRAWER_WIDTH, bgcolor: 'chrome.main', color: 'chrome.contrastText',
          },
        }}
      >
        <Toolbar />
        <List>
          <ListItemButton
            component={NavLink} to="/admin/dashboard"
            sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
          >
            <ListItemText primary="Dashboard" />
          </ListItemButton>
          <ListItemButton
            component={NavLink} to="/admin/categorias"
            sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
          >
            <ListItemText primary="Categorias" />
          </ListItemButton>
          <ListItemButton
            component={NavLink} to="/admin/palavras"
            sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
          >
            <ListItemText primary="Palavras" />
          </ListItemButton>
          <ListItemButton
            component={NavLink} to="/admin/sobre-site"
            sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
          >
            <ListItemText primary="Sobre o site" />
          </ListItemButton>
        </List>
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
};

import { useState } from 'react';
import { Outlet, useNavigate, NavLink } from 'react-router-dom';
import { AppBar, Box, Button, Drawer, IconButton, List, ListItemButton, ListItemText, Toolbar, Typography } from '@mui/material';
import { Menu as MenuIcon } from '@mui/icons-material';
import { alpha } from '@mui/material/styles';
import { useAuth } from '../../contexts/AuthContext';

const DRAWER_WIDTH = 220;

export const AdminLayout = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [mobileOpen, setMobileOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navItems = (
    <List>
      <ListItemButton
        component={NavLink} to="/admin/dashboard" onClick={() => setMobileOpen(false)}
        sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
      >
        <ListItemText primary="Dashboard" />
      </ListItemButton>
      <ListItemButton
        component={NavLink} to="/admin/categorias" onClick={() => setMobileOpen(false)}
        sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
      >
        <ListItemText primary="Categorias" />
      </ListItemButton>
      <ListItemButton
        component={NavLink} to="/admin/palavras" onClick={() => setMobileOpen(false)}
        sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
      >
        <ListItemText primary="Palavras" />
      </ListItemButton>
      <ListItemButton
        component={NavLink} to="/admin/sobre-site" onClick={() => setMobileOpen(false)}
        sx={{ '&:hover': { bgcolor: (theme) => alpha(theme.palette.chrome.contrastText, 0.08) } }}
      >
        <ListItemText primary="Sobre o site" />
      </ListItemButton>
    </List>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1, bgcolor: 'chrome.main', color: 'chrome.contrastText' }}>
        <Toolbar sx={{ justifyContent: 'space-between' }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <IconButton
              color="inherit" edge="start" onClick={() => setMobileOpen(true)}
              sx={{ display: { md: 'none' } }}
            >
              <MenuIcon />
            </IconButton>
            <Typography variant="h6" component="div">AdedonhaAPI — Admin</Typography>
          </Box>
          <Button color="inherit" onClick={handleLogout}>Sair</Button>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={() => setMobileOpen(false)}
        ModalProps={{ keepMounted: true }}
        sx={{
          display: { xs: 'block', md: 'none' },
          '& .MuiDrawer-paper': { width: DRAWER_WIDTH, bgcolor: 'chrome.main', color: 'chrome.contrastText' },
        }}
      >
        <Toolbar />
        {navItems}
      </Drawer>

      <Drawer
        variant="permanent"
        sx={{
          display: { xs: 'none', md: 'block' },
          width: DRAWER_WIDTH, flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: DRAWER_WIDTH, bgcolor: 'chrome.main', color: 'chrome.contrastText',
          },
        }}
      >
        <Toolbar />
        {navItems}
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1, p: 3, width: { xs: '100%', md: `calc(100% - ${DRAWER_WIDTH}px)` } }}>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
};

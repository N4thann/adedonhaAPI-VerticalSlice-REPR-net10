import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material';

interface ConfirmDialogProps {
  open: boolean;
  title: string;
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export const ConfirmDialog = ({ open, title, message, onConfirm, onCancel, isLoading }: ConfirmDialogProps) => (
  <Dialog open={open} onClose={onCancel}>
    <DialogTitle>{title}</DialogTitle>
    <DialogContent>
      <DialogContentText>{message}</DialogContentText>
    </DialogContent>
    <DialogActions>
      <Button onClick={onCancel} disabled={isLoading}>Cancelar</Button>
      <Button onClick={onConfirm} color="error" variant="contained" disabled={isLoading}>Excluir</Button>
    </DialogActions>
  </Dialog>
);

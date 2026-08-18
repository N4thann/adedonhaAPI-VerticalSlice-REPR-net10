import { useState, type ChangeEvent } from 'react';
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, List, ListItem, ListItemText, Typography } from '@mui/material';
import { wordService } from '../../services/wordService';
import type { BulkUploadResult } from '../../types/word.types';

interface CsvImportDialogProps {
  open: boolean;
  onClose: () => void;
  onImported: () => void;
}

export const CsvImportDialog = ({ open, onClose, onImported }: CsvImportDialogProps) => {
  const [file, setFile] = useState<File | undefined>(undefined);
  const [result, setResult] = useState<BulkUploadResult | undefined>(undefined);
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    setFile(event.target.files?.[0]);
    setResult(undefined);
    setError(null);
  };

  const handleUpload = async () => {
    if (!file) return;
    setIsUploading(true);
    setError(null);
    try {
      const uploadResult = await wordService.bulkUploadCsv(file);
      setResult(uploadResult);
      onImported();
    } catch {
      setError('Falha ao importar o arquivo CSV.');
    } finally {
      setIsUploading(false);
    }
  };

  const handleClose = () => {
    setFile(undefined);
    setResult(undefined);
    setError(null);
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Importar palavras via CSV</DialogTitle>
      <DialogContent>
        <Box sx={{ mb: 2 }}>
          <Button variant="outlined" component="label">
            Escolher arquivo .csv
            <input type="file" accept=".csv" hidden onChange={handleFileChange} />
          </Button>
          {file && <Typography variant="body2" sx={{ mt: 1 }}>{file.name}</Typography>}
        </Box>

        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        {result && (
          <Box>
            <Alert severity="success" sx={{ mb: 2 }}>
              {result.wordsCreated} palavra(s) criada(s), {result.categoriesCreated} categoria(s) criada(s), {' '}
              {result.associationsCreated} associação(ões) criada(s), {result.rowsSkipped} linha(s) pulada(s) de {result.totalRows}.
            </Alert>
            {result.errors.length > 0 && (
              <List dense sx={{ maxHeight: 200, overflow: 'auto' }}>
                {result.errors.map((rowError, index) => (
                  <ListItem key={index}>
                    <ListItemText primary={`Linha ${rowError.line}`} secondary={rowError.reason} />
                  </ListItem>
                ))}
              </List>
            )}
          </Box>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Fechar</Button>
        <Button onClick={handleUpload} variant="contained" disabled={!file || isUploading}>
          {isUploading ? 'Enviando...' : 'Enviar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

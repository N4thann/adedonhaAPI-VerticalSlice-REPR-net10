import { useEffect, useState, type ChangeEvent, type FormEvent } from 'react';
import { Alert, Avatar, Box, Button, CircularProgress, Paper, TextField, Typography } from '@mui/material';
import { Save } from '@mui/icons-material';
import { StringListEditor } from '../../../components/molecules/StringListEditor';
import { useAboutSiteStore } from '../../../store/aboutSite/useAboutSiteStore';
import { resolveUploadUrl } from '../../../services/api';

export const AdminAboutSitePage = () => {
  const { data, fetchAboutSite, saveAboutSite } = useAboutSiteStore();
  const [cargo, setCargo] = useState('');
  const [textoGeral, setTextoGeral] = useState('');
  const [formacoes, setFormacoes] = useState<string[]>([]);
  const [tecnologias, setTecnologias] = useState<string[]>([]);
  const [arquiteturas, setArquiteturas] = useState<string[]>([]);
  const [image, setImage] = useState<File | undefined>(undefined);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    fetchAboutSite();
  }, [fetchAboutSite]);

  useEffect(() => {
    if (data) {
      setCargo(data.cargo);
      setTextoGeral(data.textoGeral);
      setFormacoes(data.formacoes);
      setTecnologias(data.tecnologias);
      setArquiteturas(data.arquiteturas);
    }
  }, [data]);

  const handleImageChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;
    setImage(file);
    setImagePreview(URL.createObjectURL(file));
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);
    setIsSaving(true);
    try {
      await saveAboutSite({ cargo, textoGeral, formacoes, tecnologias, arquiteturas, image });
      setSuccess(true);
      setTimeout(() => setSuccess(false), 3000);
    } catch {
      setError('Erro ao salvar o Sobre o site.');
    } finally {
      setIsSaving(false);
    }
  };

  const displayedImage = imagePreview ?? resolveUploadUrl(data?.imageUrl ?? null) ?? undefined;

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>Sobre o site</Typography>

      <Paper sx={{ p: 4 }}>
        <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          {error && <Alert severity="error">{error}</Alert>}
          {success && <Alert severity="success">Salvo com sucesso!</Alert>}

          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Avatar src={displayedImage} sx={{ width: 80, height: 80 }} />
            <Button variant="outlined" component="label">
              Escolher foto
              <input type="file" accept="image/*" hidden onChange={handleImageChange} />
            </Button>
          </Box>

          <TextField label="Cargo" value={cargo} onChange={(e) => setCargo(e.target.value)} fullWidth />

          <StringListEditor itemLabel="Formação" value={formacoes} onChange={setFormacoes} />
          <StringListEditor itemLabel="Tecnologia" value={tecnologias} onChange={setTecnologias} />
          <StringListEditor itemLabel="Arquitetura" value={arquiteturas} onChange={setArquiteturas} />

          <TextField
            label="Texto geral" value={textoGeral} onChange={(e) => setTextoGeral(e.target.value)}
            multiline minRows={4} fullWidth
          />

          <Button
            type="submit" variant="contained" disabled={isSaving}
            startIcon={isSaving ? <CircularProgress size={20} color="inherit" /> : <Save />}
            sx={{ alignSelf: 'flex-end' }}
          >
            {isSaving ? 'Salvando...' : 'Salvar'}
          </Button>
        </Box>
      </Paper>
    </Box>
  );
};

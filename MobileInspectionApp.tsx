import React, { useState } from 'react';
import {
  Container,
  Box,
  TextField,
  Button,
  Typography,
  Card,
  CardContent,
  LinearProgress,
  Checkbox,
  FormControlLabel,
  IconButton,
  BottomNavigation,
  BottomNavigationAction,
  Paper,
  Stepper,
  Step,
  StepLabel,
  Chip,
  Grid,
  Avatar,
  Divider,
  Alert,
  CircularProgress,
  Stack,
  Radio,
  RadioGroup,
  FormControl,
  FormLabel,
  Rating,
  Switch,
  Slider,
} from '@mui/material';
import {
  Login as LoginIcon,
  Home as HomeIcon,
  Route as RouteIcon,
  Assessment as AssessmentIcon,
  Person as PersonIcon,
  ArrowBack as ArrowBackIcon,
  CheckCircle as CheckCircleIcon,
  Warning as WarningIcon,
  PhotoCamera as PhotoCameraIcon,
  Download as DownloadIcon,
  Settings as SettingsIcon,
  Error as ErrorIcon,
} from '@mui/icons-material';
import { ThemeProvider, createTheme } from '@mui/material/styles';

// Тема для мобильного приложения
const theme = createTheme({
  palette: {
    primary: {
      main: '#2196f3',
    },
    secondary: {
      main: '#ff9800',
    },
    error: {
      main: '#f44336',
    },
    success: {
      main: '#4caf50',
    },
  },
  typography: {
    fontFamily: 'system-ui, -apple-system, sans-serif',
  },
});

// =====================================================
// ЭКРАН 1: АВТОРИЗАЦИЯ
// =====================================================
const LoginScreen = ({ onLogin }: { onLogin: () => void }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: '#f5f5f5', p: 3 }}>
      <Card sx={{ width: '100%', maxWidth: 360, borderRadius: 4, overflow: 'hidden' }}>
        <Box sx={{ bgcolor: 'primary.main', p: 4, textAlign: 'center' }}>
          <Avatar sx={{ width: 64, height: 64, mx: 'auto', mb: 2, bgcolor: 'white' }}>
            <LoginIcon sx={{ fontSize: 40, color: 'primary.main' }} />
          </Avatar>
          <Typography variant="h5" sx={{ color: 'white', fontWeight: 'bold' }}>
            AgroControl
          </Typography>
          <Typography variant="body2" sx={{ color: 'white', opacity: 0.8 }}>
            Обход оборудования
          </Typography>
        </Box>
        <Box sx={{ p: 3 }}>
          <TextField
            fullWidth
            label="Логин"
            variant="outlined"
            margin="normal"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Введите логин"
          />
          <TextField
            fullWidth
            label="Пароль"
            type="password"
            variant="outlined"
            margin="normal"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Введите пароль"
          />
          <Button
            fullWidth
            variant="contained"
            size="large"
            sx={{ mt: 3, mb: 2, py: 1.5, borderRadius: 2 }}
            onClick={onLogin}
          >
            Войти
          </Button>
          <Typography variant="body2" align="center" sx={{ color: 'text.secondary' }}>
            Забыли пароль? Обратитесь к администратору
          </Typography>
        </Box>
      </Card>
    </Box>
  );
};

// =====================================================
// ЭКРАН 2: СПИСОК МАРШРУТОВ ОБХОДА
// =====================================================
const RoutesScreen = ({ onSelectRoute }: { onSelectRoute: (routeId: string) => void }) => {
  const routes = [
    { id: '1', name: 'Линия экструдера', status: 'in_progress', progress: 40, icon: '🏭' },
    { id: '2', name: 'Склад сырья', status: 'not_started', progress: 0, icon: '📦' },
    { id: '3', name: 'Лабораторный корпус', status: 'completed', progress: 100, icon: '🔬' },
    { id: '4', name: 'Упаковочная линия', status: 'not_started', progress: 0, icon: '📦' },
  ];

  const getStatusChip = (status: string) => {
    switch (status) {
      case 'in_progress':
        return <Chip label="В процессе" color="primary" size="small" />;
      case 'completed':
        return <Chip label="Завершён" color="success" size="small" />;
      default:
        return <Chip label="Не начат" color="default" size="small" />;
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f5f5f5', pb: 8 }}>
      <Box sx={{ bgcolor: 'primary.main', p: 3, pt: 5, color: 'white' }}>
        <Typography variant="h5" fontWeight="bold">
          Мои маршруты
        </Typography>
        <Typography variant="body2" sx={{ opacity: 0.8 }}>
          Выберите маршрут для осмотра
        </Typography>
      </Box>

      <Container sx={{ pt: 2, pb: 4 }}>
        <Stack spacing={2}>
          {routes.map((route) => (
            <Card
              key={route.id}
              sx={{
                borderRadius: 3,
                cursor: 'pointer',
                transition: 'transform 0.2s',
                '&:hover': { transform: 'scale(1.01)' },
              }}
              onClick={() => onSelectRoute(route.id)}
            >
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                  <Typography variant="h4" sx={{ mr: 2 }}>
                    {route.icon}
                  </Typography>
                  <Box sx={{ flex: 1 }}>
                    <Typography variant="h6" fontWeight="bold">
                      {route.name}
                    </Typography>
                    {getStatusChip(route.status)}
                  </Box>
                  <Button
                    variant="contained"
                    size="small"
                    sx={{ borderRadius: 2 }}
                    onClick={(e) => {
                      e.stopPropagation();
                      onSelectRoute(route.id);
                    }}
                  >
                    {route.status === 'in_progress' ? 'Продолжить' : 
                     route.status === 'completed' ? 'Смотреть' : 'Начать'}
                  </Button>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={route.progress}
                  sx={{ mt: 2, borderRadius: 1, height: 6 }}
                />
                <Typography variant="caption" sx={{ color: 'text.secondary', mt: 1, display: 'block' }}>
                  Выполнено {route.progress}%
                </Typography>
              </CardContent>
            </Card>
          ))}
        </Stack>
      </Container>

      <Paper sx={{ position: 'fixed', bottom: 0, left: 0, right: 0, borderRadius: 0 }} elevation={3}>
        <BottomNavigation showLabels value="routes">
          <BottomNavigationAction label="Главная" icon={<HomeIcon />} />
          <BottomNavigationAction label="Маршруты" icon={<RouteIcon />} selected />
          <BottomNavigationAction label="Отчеты" icon={<AssessmentIcon />} />
          <BottomNavigationAction label="Профиль" icon={<PersonIcon />} />
        </BottomNavigation>
      </Paper>
    </Box>
  );
};

// =====================================================
// ЭКРАН 3: СПИСОК КОНТРОЛЬНЫХ ТОЧЕК
// =====================================================
const CheckpointsScreen = ({ onBack, onSelectPoint }: { onBack: () => void; onSelectPoint: (pointId: number) => void }) => {
  const [checkedPoints, setCheckedPoints] = useState<number[]>([1]);
  const points = [
    { id: 1, name: 'Осмотр экструдера', status: 'completed', mandatory: true },
    { id: 2, name: 'Проверка давления', status: 'pending', mandatory: true },
    { id: 3, name: 'Замер температуры', status: 'pending', mandatory: true },
    { id: 4, name: 'Визуальный осмотр', status: 'pending', mandatory: false },
    { id: 5, name: 'Смазка подшипников', status: 'pending', mandatory: true },
  ];

  const progress = (checkedPoints.length / points.length) * 100;

  const handleCheck = (pointId: number) => {
    if (checkedPoints.includes(pointId)) {
      setCheckedPoints(checkedPoints.filter(id => id !== pointId));
    } else {
      setCheckedPoints([...checkedPoints, pointId]);
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f5f5f5', pb: 8 }}>
      <Box sx={{ bgcolor: 'primary.main', p: 3, pt: 5, color: 'white' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <IconButton sx={{ color: 'white', mr: 1 }} onClick={onBack}>
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h6" sx={{ flex: 1 }}>
            Линия экструдера
          </Typography>
        </Box>
        <Typography variant="body2" sx={{ opacity: 0.8, mb: 1 }}>
          Прогресс обхода
        </Typography>
        <LinearProgress variant="determinate" value={progress} sx={{ borderRadius: 2, height: 8, bgcolor: 'rgba(255,255,255,0.3)' }} />
        <Typography variant="caption" sx={{ mt: 1, display: 'block', opacity: 0.8 }}>
          {checkedPoints.length} из {points.length} точек осмотрено
        </Typography>
      </Box>

      <Container sx={{ pt: 2 }}>
        <Stack spacing={2}>
          {points.map((point) => (
            <Card
              key={point.id}
              sx={{
                borderRadius: 2,
                opacity: checkedPoints.includes(point.id) ? 0.7 : 1,
                cursor: 'pointer',
              }}
              onClick={() => onSelectPoint(point.id)}
            >
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Checkbox
                      checked={checkedPoints.includes(point.id)}
                      onChange={() => handleCheck(point.id)}
                      onClick={(e) => e.stopPropagation()}
                      disabled={point.status === 'completed'}
                    />
                    <Box>
                      <Typography variant="body1" fontWeight={500}>
                        {point.name}
                      </Typography>
                      {point.mandatory && (
                        <Chip label="Обязательно" size="small" color="error" variant="outlined" sx={{ mt: 0.5 }} />
                      )}
                    </Box>
                  </Box>
                  {checkedPoints.includes(point.id) && <CheckCircleIcon color="success" />}
                </Box>
              </CardContent>
            </Card>
          ))}
        </Stack>
      </Container>

      <Paper sx={{ position: 'fixed', bottom: 0, left: 0, right: 0, borderRadius: 0, p: 2, bgcolor: 'white' }} elevation={3}>
        <Button
          fullWidth
          variant="contained"
          size="large"
          disabled={checkedPoints.length < points.filter(p => p.mandatory).length}
          sx={{ borderRadius: 2, py: 1.5 }}
        >
          Завершить обход
        </Button>
      </Paper>
    </Box>
  );
};

// =====================================================
// ЭКРАН 4: КАРТОЧКА ТОЧКИ ОСМОТРА
// =====================================================
const PointInspectionScreen = ({ onBack, onComplete, onReportIssue }: { onBack: () => void; onComplete: () => void; onReportIssue: () => void }) => {
  const [temperature, setTemperature] = useState<number>(85);
  const [pressure, setPressure] = useState<number>(3.5);
  const [visualCheck, setVisualCheck] = useState(false);
  const [comment, setComment] = useState('');

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f5f5f5' }}>
      <Box sx={{ bgcolor: 'primary.main', p: 3, pt: 5, color: 'white' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <IconButton sx={{ color: 'white', mr: 1 }} onClick={onBack}>
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h6">Точка 3: Замер температуры</Typography>
        </Box>
      </Box>

      <Container sx={{ pt: 3 }}>
        <Card sx={{ borderRadius: 3, p: 2, mb: 2 }}>
          <Typography variant="subtitle2" color="text.secondary" gutterBottom>
            Показания экструдера
          </Typography>
          
          <Box sx={{ mt: 2 }}>
            <Typography variant="body2" gutterBottom>Температура, °C</Typography>
            <TextField
              fullWidth
              type="number"
              value={temperature}
              onChange={(e) => setTemperature(Number(e.target.value))}
              InputProps={{ endAdornment: <Typography>°C</Typography> }}
              sx={{ mb: 3 }}
            />
            <Slider
              value={temperature}
              onChange={(_, val) => setTemperature(val as number)}
              min={60}
              max={100}
              marks
              valueLabelDisplay="auto"
              sx={{ mb: 3 }}
            />
          </Box>

          <Box sx={{ mt: 2 }}>
            <Typography variant="body2" gutterBottom>Давление, бар</Typography>
            <TextField
              fullWidth
              type="number"
              value={pressure}
              onChange={(e) => setPressure(Number(e.target.value))}
              InputProps={{ endAdornment: <Typography>бар</Typography> }}
            />
            <Slider
              value={pressure}
              onChange={(_, val) => setPressure(val as number)}
              min={2}
              max={5}
              step={0.1}
              valueLabelDisplay="auto"
            />
          </Box>

          <FormControlLabel
            control={<Checkbox checked={visualCheck} onChange={(e) => setVisualCheck(e.target.checked)} />}
            label="Визуальный осмотр пройден (нет трещин, подтёков)"
            sx={{ mt: 3 }}
          />

          <TextField
            fullWidth
            label="Замечание (при наличии)"
            multiline
            rows={3}
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Опишите обнаруженные проблемы..."
            sx={{ mt: 2 }}
          />

          <Divider sx={{ my: 3 }} />

          <Button
            fullWidth
            variant="contained"
            size="large"
            onClick={onComplete}
            sx={{ mb: 2, py: 1.5, borderRadius: 2 }}
          >
            Завершить осмотр точки
          </Button>

          <Button
            fullWidth
            variant="outlined"
            color="error"
            startIcon={<WarningIcon />}
            onClick={onReportIssue}
            sx={{ py: 1.5, borderRadius: 2 }}
          >
            Сообщить о проблеме
          </Button>
        </Card>
      </Container>
    </Box>
  );
};

// =====================================================
// ЭКРАН 5: РЕГИСТРАЦИЯ ЗАМЕЧАНИЯ
// =====================================================
const ReportIssueScreen = ({ onBack, onSubmit }: { onBack: () => void; onSubmit: () => void }) => {
  const [description, setDescription] = useState('');

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f5f5f5' }}>
      <Box sx={{ bgcolor: 'error.main', p: 3, pt: 5, color: 'white' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <IconButton sx={{ color: 'white', mr: 1 }} onClick={onBack}>
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h6">Сообщить о проблеме</Typography>
        </Box>
      </Box>

      <Container sx={{ pt: 3 }}>
        <Alert severity="warning" sx={{ mb: 3, borderRadius: 2 }}>
          Внимание! Критические отклонения могут привести к остановке производства
        </Alert>

        <Card sx={{ borderRadius: 3, p: 2 }}>
          <Typography variant="h6" gutterBottom>Опишите проблему</Typography>
          <TextField
            fullWidth
            multiline
            rows={5}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Подробно опишите неисправность, отклонение параметров или другую проблему..."
            sx={{ mb: 2 }}
          />
          
          <Box
            sx={{
              border: '2px dashed #ccc',
              borderRadius: 2,
              p: 3,
              textAlign: 'center',
              cursor: 'pointer',
              mb: 2,
              '&:hover': { borderColor: 'primary.main' },
            }}
          >
            <PhotoCameraIcon sx={{ fontSize: 40, color: '#999' }} />
            <Typography variant="body2" color="text.secondary">
              Добавить фото (опционально)
            </Typography>
          </Box>

          <FormControlLabel
            control={<Switch defaultChecked />}
            label="Уведомить технолога немедленно"
            sx={{ mt: 1 }}
          />

          <Button
            fullWidth
            variant="contained"
            color="error"
            size="large"
            onClick={onSubmit}
            disabled={!description}
            sx={{ mt: 3, py: 1.5, borderRadius: 2 }}
          >
            Отправить замечание
          </Button>

          <Button
            fullWidth
            variant="text"
            onClick={onBack}
            sx={{ mt: 1 }}
          >
            Отмена
          </Button>
        </Card>
      </Container>
    </Box>
  );
};

// =====================================================
// ЭКРАН 6: ИТОГОВЫЙ ОТЧЕТ
// =====================================================
const ReportScreen = ({ onBack }: { onBack: () => void }) => {
  const stats = [
    { label: 'Осмотрено точек', value: '8 / 8', icon: <CheckCircleIcon color="success" /> },
    { label: 'Замечаний', value: '2', icon: <WarningIcon color="warning" /> },
    { label: 'Критических', value: '0', icon: <ErrorIcon color="error" /> },
    { label: 'Время обхода', value: '42 мин', icon: <AssessmentIcon /> },
  ];

  const details = [
    { point: 'Осмотр экструдера', result: '✅', comment: 'Норма' },
    { point: 'Проверка давления', result: '⚠️', comment: 'Незначительное отклонение' },
    { point: 'Замер температуры', result: '✅', comment: '84°C (норма 80-90)' },
    { point: 'Визуальный осмотр', result: '❌', comment: 'Обнаружена трещина на корпусе' },
    { point: 'Смазка подшипников', result: '✅', comment: 'Выполнена' },
  ];

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f5f5f5', pb: 8 }}>
      <Box sx={{ bgcolor: 'success.main', p: 3, pt: 5, color: 'white', textAlign: 'center' }}>
        <CheckCircleIcon sx={{ fontSize: 64, mb: 1 }} />
        <Typography variant="h5" fontWeight="bold">Обход завершён!</Typography>
        <Typography variant="body2" sx={{ opacity: 0.9 }}>25 марта 2025 г. 14:30</Typography>
      </Box>

      <Container sx={{ pt: 3 }}>
        <Grid container spacing={2} sx={{ mb: 3 }}>
          {stats.map((stat, idx) => (
            <Grid item xs={6} key={idx}>
              <Card sx={{ textAlign: 'center', p: 2, borderRadius: 2 }}>
                <Box sx={{ display: 'flex', justifyContent: 'center', mb: 1 }}>
                  {stat.icon}
                </Box>
                <Typography variant="h4" fontWeight="bold">
                  {stat.value}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {stat.label}
                </Typography>
              </Card>
            </Grid>
          ))}
        </Grid>

        <Typography variant="h6" gutterBottom>Детализация по точкам</Typography>
        
        <Card sx={{ borderRadius: 2, overflow: 'hidden', mb: 3 }}>
          {details.map((item, idx) => (
            <Box key={idx}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', p: 2 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <Typography variant="body1">{item.result}</Typography>
                  <Typography variant="body2">{item.point}</Typography>
                </Box>
                <Typography variant="caption" color="text.secondary">
                  {item.comment}
                </Typography>
              </Box>
              {idx < details.length - 1 && <Divider />}
            </Box>
          ))}
        </Card>

        <Button
          fullWidth
          variant="outlined"
          startIcon={<DownloadIcon />}
          sx={{ mb: 2, py: 1.5, borderRadius: 2 }}
        >
          Экспорт в Excel
        </Button>

        <Button
          fullWidth
          variant="contained"
          onClick={onBack}
          sx={{ py: 1.5, borderRadius: 2 }}
        >
          На главную
        </Button>
      </Container>
    </Box>
  );
};

// =====================================================
// ГЛАВНЫЙ КОМПОНЕНТ (управление навигацией)
// =====================================================
const App = () => {
  const [screen, setScreen] = useState<'login' | 'routes' | 'checkpoints' | 'inspection' | 'reportIssue' | 'report'>('login');
  const [selectedRoute, setSelectedRoute] = useState<string | null>(null);

  return (
    <ThemeProvider theme={theme}>
      {screen === 'login' && <LoginScreen onLogin={() => setScreen('routes')} />}
      {screen === 'routes' && <RoutesScreen onSelectRoute={(id) => { setSelectedRoute(id); setScreen('checkpoints'); }} />}
      {screen === 'checkpoints' && <CheckpointsScreen onBack={() => setScreen('routes')} onSelectPoint={() => setScreen('inspection')} />}
      {screen === 'inspection' && <PointInspectionScreen onBack={() => setScreen('checkpoints')} onComplete={() => setScreen('report')} onReportIssue={() => setScreen('reportIssue')} />}
      {screen === 'reportIssue' && <ReportIssueScreen onBack={() => setScreen('inspection')} onSubmit={() => setScreen('inspection')} />}
      {screen === 'report' && <ReportScreen onBack={() => setScreen('routes')} />}
    </ThemeProvider>
  );
};

export default App;
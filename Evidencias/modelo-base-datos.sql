-- Propuesta futura. La version actual usa InMemoryStore.
CREATE TABLE asociados (
  id UUID PRIMARY KEY,
  numero_documento VARCHAR(30) NOT NULL UNIQUE,
  nombre_completo VARCHAR(150) NOT NULL,
  telefono VARCHAR(30), direccion VARCHAR(200),
  fecha_registro TIMESTAMP NOT NULL,
  estado VARCHAR(20) NOT NULL DEFAULT 'ACTIVO'
);
CREATE TABLE usuarios (
  id UUID PRIMARY KEY,
  nombre VARCHAR(150) NOT NULL,
  rol VARCHAR(30) NOT NULL,
  activo BOOLEAN NOT NULL DEFAULT TRUE
);
CREATE TABLE tipos_movimiento (
  id INT PRIMARY KEY,
  codigo VARCHAR(20) NOT NULL UNIQUE,
  nombre VARCHAR(50) NOT NULL
);
CREATE TABLE movimientos (
  id UUID PRIMARY KEY,
  asociado_id UUID NOT NULL REFERENCES asociados(id),
  usuario_id UUID REFERENCES usuarios(id),
  tipo_id INT NOT NULL REFERENCES tipos_movimiento(id),
  monto DECIMAL(18,2) NOT NULL CHECK (monto > 0),
  comision DECIMAL(18,2) NOT NULL DEFAULT 0,
  fecha TIMESTAMP NOT NULL
);
CREATE TABLE trm (
  id UUID PRIMARY KEY,
  valor DECIMAL(18,4) NOT NULL,
  vigencia_desde DATE NOT NULL,
  vigencia_hasta DATE NOT NULL,
  consultada_en TIMESTAMP NOT NULL
);
CREATE TABLE consultas_trm (
  id UUID PRIMARY KEY,
  asociado_id UUID NOT NULL REFERENCES asociados(id),
  trm_id UUID NOT NULL REFERENCES trm(id),
  saldo_cop DECIMAL(18,2) NOT NULL,
  saldo_usd DECIMAL(18,2) NOT NULL,
  consultada_en TIMESTAMP NOT NULL
);

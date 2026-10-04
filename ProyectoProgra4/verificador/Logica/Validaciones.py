#validaciones de datos del verificador 
import re 
import unicodedata

#Datos de referencia 

#249 codigos oficial ISO 3166-1 alfa-2
PAISES_ISO = frozenset("""
AD AE AF AG AI AL AM AO AQ AR AS AT AU AW AX AZ BA BB BD BE BF BG BH BI
BJ BL BM BN BO BQ BR BS BT BV BW BY BZ CA CC CD CF CG CH CI CK CL CM CN
CO CR CU CV CW CX CY CZ DE DJ DK DM DO DZ EC EE EG EH ER ES ET FI FJ FK
FM FO FR GA GB GD GE GF GG GH GI GL GM GN GP GQ GR GS GT GU GW GY HK HM
HN HR HT HU ID IE IL IM IN IO IQ IR IS IT JE JM JO JP KE KG KH KI KM KN
KP KR KW KY KZ LA LB LC LI LK LR LS LT LU LV LY MA MC MD ME MF MG MH MK
ML MM MN MO MP MQ MR MS MT MU MV MW MX MY MZ NA NC NE NF NG NI NL NO NP
NR NU NZ OM PA PE PF PG PH PK PL PM PN PR PS PT PW PY QA RE RO RS RU RW
SA SB SC SD SE SG SH SI SJ SK SL SM SN SO SR SS ST SV SX SY SZ TC TD TF
TG TH TJ TK TL TM TN TO TR TT TV TW TZ UA UG UM US UY UZ VA VC VE VG VI
VN VU WF WS YE YT ZA ZM ZW
""".split())
 
PROVINCIAS_CR = ("san jose", "alajuela", "cartago", "heredia",
                 "guanacaste", "puntarenas", "limon")
 
#ajuste varchar de tabla cliente
LARGO_NOMBRE = 20
LARGO_CORREO = 100

 
#expresiones regulares 
 
_LETRAS = "A-Za-zÁÉÍÓÚÜÑáéíóúüñ"
_RE_NOMBRE = re.compile(rf"[{_LETRAS}]+(?:[ '-][{_LETRAS}]+)*")
_RE_CORREO = re.compile(r"[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}")
_RE_TELEFONO = re.compile(r"(?:\+?506[ -]?)?([245678][0-9]{3})[ -]?([0-9]{4})")
_RE_CEDULA_FISICA = re.compile(r"[1-9][0-9]{8}")       # 9 dígitos
_RE_CEDULA_JURIDICA = re.compile(r"3[0-9]{9}")         # 10 dígitos, empieza en 3
_RE_EXTRANJERO = re.compile(r"[A-Za-z0-9]{6,20}")      # DIMEX o pasaporte

#funciones
def es_texto(valor):
    """true si es un string con algo mas que espacios"""
    return isinstance(valor, str) and valor.strip() != ""
    
def pais_valido(pais):
    """codigo de dos letras en ISO 3166-1 alfa-2"""
    return pais in PAISES_ISO

def identificacion_valida(identificacion, pais):
    """Cedula fisica valida para CR , cedula fisica o juridica, otro pais (dimex o pasaporte)"""
    if pais == "CR": 
        return bool(_RE_CEDULA_FISICA.fullmatch(identificacion)
                    or _RE_CEDULA_JURIDICA.fullmatch(identificacion))
    return bool(_RE_EXTRANJERO.fullmatch(identificacion))
    
def nombre_valido(texto):
    """solo letras, se permiten tildes, nombres compuestos, sin numeros"""
    return len(texto) <= LARGO_NOMBRE and bool(_RE_NOMBRE.fullmatch(texto))

def correo_valido(correo):
    """se valida correo con su formato correcto @, .ext"""
    return len(correo) <= LARGO_CORREO and bool(_RE_CORREO.fullmatch(correo))

def normalizar_telefono(telefono):
    """Acepta numeros de telefono los convierte y devuelve limpios 8 digitos,none si no es de CR"""
    coincidencia = _RE_TELEFONO.fullmatch(telefono)
    if coincidencia is None:
        return None
    return coincidencia.group(1) + coincidencia.group(2)

def direccion_valida(direccion):
    """comprueba si es texto no vacio"""
    return es_texto(direccion)




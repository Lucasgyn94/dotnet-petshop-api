
namespace Api;

using static BCrypt.Net.BCrypt;

public class AdministradorServico : IAdministradorServico
{

    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto)
    {
        this._contexto = contexto;
    }


    public Administrador BuscarPorId(int id)
    {
        return this._contexto.Administradores.Where(a => a.Id == id).FirstOrDefault()!;
    }

    public Administrador Incluir(Administrador administrador)
    {
        administrador.Senha = HashPassword(administrador.Senha, workFactor: 11);

        this._contexto.Administradores.Add(administrador);
        this._contexto.SaveChanges();
        return administrador;
    }

    public Administrador Login(LoginDTO loginDTO)
    {
        var administrador = this._contexto.Administradores.FirstOrDefault(a => a.Email == loginDTO.Email);

        if (administrador == null)
        {
            return null!;
        }

        if (Verify(loginDTO.Senha, administrador.Senha))
        {
            return administrador;
        }
        return null!;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var consulta = this._contexto.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
        {
            consulta = consulta.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        return consulta.ToList();
    }

}

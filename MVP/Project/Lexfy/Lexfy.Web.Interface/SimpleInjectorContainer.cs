using Lexfy.Application;
using Lexfy.Application.Identity;
using Lexfy.Application.Identity.Interfaces;
using Lexfy.Application.Interfaces;
using Lexfy.Repository;
using Lexfy.Repository.Identity;
using Lexfy.Repository.Identity.Interfaces;
using Lexfy.Repository.Interfaces;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Lifestyles;
using System.Web;

namespace Lexfy.Web.Interface
{
    public class SimpleInjectorContainer
    {
        public static Container RegisterServices()
        {
            var container = new Container();

            var webLifeStyle = Lifestyle.CreateHybrid(
                                    lifestyleSelector: () => HttpContext.Current != null,
                                    trueLifestyle: new WebRequestLifestyle(),
                                    falseLifestyle: new ThreadScopedLifestyle());

            // registrando as implementações
            container.Register<ITreeApplication, TreeApplication>(webLifeStyle);
            container.Register<ITreeRepository, TreeRepository>(webLifeStyle);

            container.Register<IBranchApplication, BranchApplication>(webLifeStyle);
            container.Register<IBranchRepository, BranchRepository>(webLifeStyle);

            container.Register<INodeApplication, NodeApplication>(webLifeStyle);
            container.Register<INodeRepository, NodeRepository>(webLifeStyle);

            container.Register<ITagApplication, TagApplication>(webLifeStyle);
            container.Register<ITagRepository, TagRepository>(webLifeStyle);

            container.Register<IUserApplication, UserApplication>(webLifeStyle);
            container.Register<IUserRepository, UserRepository>(webLifeStyle);

            container.Register<IUserTypeApplication, UserTypeApplication>(webLifeStyle);
            container.Register<IUserTypeRepository, UserTypeRepository>(webLifeStyle);

            container.Register<IProfileApplication, ProfileApplication>(webLifeStyle);
            container.Register<IProfileRepository, ProfileRepository>(webLifeStyle);

            container.Register<IAccountApplication, AccountApplication>(webLifeStyle);
            container.Register<IAccountRepository, AccountRepository>(webLifeStyle);

            container.Register<IAccountTypeApplication, AccountTypeApplication>(webLifeStyle);
            container.Register<IAccountTypeRepository, AccountTypeRepository>(webLifeStyle);

            container.Register<ICompanyApplication, CompanyApplication>(webLifeStyle);
            container.Register<ICompanyRepository, CompanyRepository>(webLifeStyle);

            container.Verify();
            return container;
        }
    }
}
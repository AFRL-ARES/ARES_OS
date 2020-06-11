using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ARESCore.Database.Filtering.Impl;
using ARESCore.Database.Tables;

namespace ARESCore.Database.Filtering
{
  interface IExperimentFilter: IDbFilter<ExperimentEntity>
  {
    List<ExperimentEntity> FilterExperiments( FilterOptions opts );
    IQueryable<ExperimentEntity> FilterExperimentsOnPlans( FilterOptions opts, IQueryable<ExperimentEntity> initExps );
    IQueryable<ExperimentEntity> FilterExperimentsOnMachine( FilterOptions opts, IQueryable<ExperimentEntity> initSet );
    IQueryable<ExperimentEntity> FilterExperimentsOnData( FilterOptions opts, IQueryable<ExperimentEntity> initSet );
  }
}

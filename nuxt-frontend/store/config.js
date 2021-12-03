export const state = () => ({
  appConfig: {},
  appLoading : true,
  userInfo: null,
  appMenu : null,
  workingCompany: {companyName: ''},
  //Items: localStorage.getItem("Items")? JSON.parse(localStorage.getItem("Items")) : [] || [],
  })
  
  export const mutations = {
    // add(state, text) {
    //   state.list.push({
    //     text,
    //     done: false
    //   })
    // },
    // remove(state, { todo }) {
    //   state.list.splice(state.list.indexOf(todo), 1)
    // },
    // toggle(state, todo) {
    //   todo.done = !todo.done
    // },
    // updateItems(state, { Items }) {
    //   state.Items =  Items;
    //   localStorage.setItem("Items", JSON.stringify(Items))
    // },
    updateAppConfig(state, { Items }) {
      console.log("store\\config\\mutations\\updateAppConfig")
      for (var p in Items){
        state.appConfig[Items[p].Parameter] = Items[p].Value
      }

      //state.appConfig =  Items;
      //localStorage.setItem("Items", JSON.stringify(Items))
    },
    updateAppLoading(state, {val}){
      console.log("store\\config\\mutations\\updateAppLoading")
      state.appLoading =  val;
    },
    updateUserInfo(state, {data}){
      console.log("store\\config\\mutations\\updateUserInfo")
      state.userInfo =  data;
    },
    updatAppMenu(state, {data}){
      console.log("store\\config\\mutations\\updatAppMenu")
      state.appMenu =  data;
    },
   
  }  

  export const getters = {
    workingCompany: (state) => state.workingCompany,
  }
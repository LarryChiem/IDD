<template>
  <div class="example-drag">
    <template v-if="onlineStatus">
      <div class="upload">
        <ul v-if="!files.length">
          <td colspan="7">
            <div class="text-center p-5">
              <h4>Drop files anywhere to upload<br />or</h4>
              <label for="file" class="btn btn-lg btn-primary"
                >Select Files</label
              >
            </div>
          </td>
        </ul>

        <div
          v-show="$refs.upload && $refs.upload.dropActive"
          class="drop-active"
        >
          <h3>Drop files to upload</h3>
        </div>

        <v-container>
          <v-row>
            <v-col>
              <div v-if="!submitted">
                <div class="example-btn">
                  <file-upload
                    class="btn btn-primary"
                    :custom-action="customAction"
                    :multiple="true"
                    :drop="true"
                    :drop-directory="true"
                    :maximum="2"
                    :size="1024 * 1024 * 10"
                    accept="image/*, application/pdf"
                    @input-file="inputFile"
                    v-model="files"
                    ref="upload"
                  >
                    <i class="fa fa-plus"></i>
                    Select files
                  </file-upload>

                  <button
                    type="button"
                    class="btn btn-success"
                    v-if="!$refs.upload || !$refs.upload.active"
                    @click.prevent="$refs.upload.active = true"
                  >
                    <i class="fa fa-arrow-up" aria-hidden="true"></i>
                    Start Upload
                  </button>

                  <button
                    type="button"
                    class="btn btn-danger"
                    v-else
                    @click.prevent="$refs.upload.active = false"
                  >
                    <i class="fa fa-stop" aria-hidden="true"></i>
                    Stop Upload
                  </button>
                </div>
              </div>
              <div v-else>
                <div class="text-center">
                  <v-btn color="red" ref="files" @click="reset">
                    Reset Files
                  </v-btn>
                </div>
              </div>

              <div v-if="files.length">
                <ul class="file-list">
                  <li v-for="file in files" :key="file.id">
                    <span data-testid="name">{{ file.name }}</span> -
                    <!--span>{{ file.size | formatSize }}</span-->
                    <span v-if="file.error">{{ file.error }}</span>
                    <span v-else-if="file.success">success</span>
                    <span v-else-if="file.active">active</span>
                    <span v-else></span>
                  </li>
                </ul>
              </div>
            </v-col>

            <div class="continue" v-if="check()">
              <v-col>
                <div class="text-right">
                  <v-btn
                    :loading="loading"
                    :disabled="loading"
                    color="success"
                    class="ma-2 white-text"
                    @click="loader = loading"
                  >
                    Complete Form
                    <v-icon right dark>mdi-cloud-upload</v-icon>
                  </v-btn>
                </div>
              </v-col>
            </div>
          </v-row>
        </v-container>
      </div>
    </template>
    <template v-else>
      OFFLINE: can't upload file unless you are online :(
    </template>
  </div>
</template>

<style lang="scss" scoped>
  .example-drag.drop-space {
    padding-inline-start: 0;
    background: #000;
  }
  .file-list {
    margin-top: 1em;
  }
  .btn-success {
    margin-left: 1rem;
  }
  .example-drag.upload {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-flow: column;
    padding-top: 20em;
  }
  .example-drag {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-flow: column;
    padding-top: 1em;
  }
  .example-drag.btn {
    margin-bottom: 0;
    margin-right: 1rem;
  }
  .example-drag.drop-active {
    top: 5em;
    bottom: 0;
    right: 0;
    left: 0;
    position: fixed;
    z-index: 9999;
    opacity: 0.6;
    text-align: center;
    background: #000;
  }
  .example-drag.drop-active h3 {
    margin: -0.5em 0 0;
    position: absolute;
    top: 10em;
    left: 5em;
    right: 0;
    -webkit-transform: translateY(-50%);
    -ms-transform: translateY(-50%);
    transform: translateY(-50%);
    font-size: 40px;
    color: #fff;
    padding: 0;
  }
  .custom-loader {
    animation: loader 1s infinite;
    display: flex;
  }
  @-moz-keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }
  @-webkit-keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }
  @-o-keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }
  @keyframes loader {
    from {
      transform: rotate(0);
    }
    to {
      transform: rotate(360deg);
    }
  }
</style>

<script>
  import { mapFields } from "vuex-map-fields";
  import FileUpload from "vue-upload-component";
  import axios from "axios";
  import { FORM } from "@/components/Utility/Enums.js";
  
  export default {
    name: "file_uploader",
    components: {
      FileUpload,
    },
    props: {
      uploadFiles: {
        type: Array,
        defaut: () => [],
        validator: (value) =>
          Array.isArray(value) &&
          value.every((file) => file.id && file.name && file.type),
      },
    },
    methods: {
      //Checks if all of the files are ready to be submitted.
      check() {
        if (!this.files.length) return false;
        let count = 0;
        let x;
        for (x in this.files) {
          if (this.files[x].success == true) count += 1;
        }
        if (count == this.files.length) {
          return true;
        } else {
          return false;
        }
      },
      reset() {
        this.files = [];
        this.submitted = false;
        this.$emit("reset");
      },
      customAction() {
        let formData = new FormData();
        for (let i = 0; i < this.files.length; i++) {
          let file = this.files[i].file;
          formData.append("files[" + i + "]", file);
        }
        formData.append("formChoice", FORM[this.formChoice]);
        let self = this;
        axios
          .post(this.urlPost, formData, {
            headers: {
              "Content-Type": "multipart/form-data",
            },
          })
          .then(function (response) {
            console.log(response);
            for (let i = 0; i < self.files.length; i++) {
              self.files[i].active = false;
              self.files[i].success = true;
            }
            self.files.active = false;
            self.files.success = true;
            self.formId = response["data"]["id"];
            self.submitted = true;
          })
          .catch(function (error) {
            console.log(error);
          });
        return;
      },
      inputFile: function (newFile, oldFile) {
        let jsonResponse;
        if (newFile.xhr) {
          if (!newFile.active) {
            jsonResponse = JSON.parse(newFile.xhr.response);
            this.formId = jsonResponse["id"];
          }
        }
        if (newFile && oldFile && !newFile.active && oldFile.active)
          if (newFile.xhr) {
            if (!newFile.active) {
              jsonResponse = JSON.parse(newFile.xhr.response);
              this.formId = jsonResponse["id"];
            }
          }
      },
    },
    data() {
      return {
        files: [],
        loader: null, //Calls our form retrieval and displays loading progress
        loading: false, //Is form retrieval loading
        submitted: false,
        getUpdated: false,
        urlGet: process.env.VUE_APP_SERVER_URL.concat(
          "Timesheet/ReadyTest?id="
        ), //Retrieve timesheet
        urlPost: process.env.VUE_APP_SERVER_URL.concat("ImageUpload/DocAsForm"), //Post AppServer
      };
    },
    computed: {
      ...mapFields(["formChoice", "formId", "onlineStatus"]),
    },
    //Watches for the user to press submit. BAD!
    watch: {
      loader() {
        const l = this.loader;
        this[l] = !this[l];
        let self = this;
        //Retrieves json response from timesheet.
        if (!this.getUpdated) {
          this.urlGet = this.urlGet.concat(this.formId);
          this.getUpdated = true;
          axios
            .get(this.urlGet)
            .then(function (response) {
              self.$emit("success", response["data"]);
            })
            .catch(function (error) {
              console.log(error);
              self.$emit("error", error);
            });
        }
        setTimeout(() => (this[l] = false), 30000);
        this.loader = null;
      },
    },
  };
</script>
